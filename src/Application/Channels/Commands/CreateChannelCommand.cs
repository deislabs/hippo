using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Config;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Application.Rules;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Channels.Commands;

public class CreateChannelCommand : IRequest<Guid>
{
    [Required]
    public Guid AppId { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public string? Domain { get; set; }

    [Required]
    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public string? RangeRule { get; set; }

    public Guid? ActiveRevisionId { get; set; }

    public Guid? CertificateId { get; set; }
}

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    private readonly HippoConfig _config;

    public CreateChannelCommandHandler(IApplicationDbContext context, HippoConfig config)
    {
        _context = context;
        _config = config;
    }

    public async Task<Guid> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {

        var app = await _context.Apps
            .Where(a => a.Id == request.AppId)
            .SingleOrDefaultAsync(cancellationToken);
        _ = app ?? throw new NotFoundException(nameof(App), request.AppId);
        var defaultDomain = $"{request.Name}.{app.Name}.{_config.PlatformDomain}"
            .Replace('_', '-')
            .ToLower();

        var entity = new Channel
        {
            AppId = request.AppId,
            App = app,
            Name = request.Name,
            Domain = (request.Domain is not null) ? request.Domain : defaultDomain,
            RevisionSelectionStrategy = request.RevisionSelectionStrategy,
            RangeRule = request.RangeRule,
            ActiveRevisionId = request.ActiveRevisionId,
            PortId = _context.Channels.Count(),
            CertificateId = request.CertificateId
        };

        if (request.CertificateId is not null)
        {
            var certificate = await _context.Certificates
                .Where(c => c.Id == request.CertificateId)
                .SingleOrDefaultAsync(cancellationToken);
            _ = certificate ?? throw new NotFoundException(nameof(Certificate), request.CertificateId);
            entity.Certificate = certificate;
        }

        if (entity.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule && entity.RangeRule is null)
        {
            entity.RangeRule = "*";
        }

        entity.ActiveRevision = await GetActiveRevision(request, entity, cancellationToken);

        entity.AddDomainEvent(new CreatedEvent<Channel>(entity));

        _context.Channels.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private async Task<Revision?> GetActiveRevision(CreateChannelCommand request, Channel entity, CancellationToken cancellationToken)
    {
        Revision? revision = null;

        if (entity.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule &&
                    request.ActiveRevisionId is null)
        {
            var revisions = _context.Revisions
                .Where(c => c.AppId == request.AppId).ToList();

            revision = RevisionRangeRule.Parse(entity.RangeRule).Match(revisions);
        }

        if (entity.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseSpecifiedRevision &&
            request.ActiveRevisionId is not null)
        {
            revision = await _context.Revisions
                .Where(c => c.Id == request.ActiveRevisionId)
                .SingleOrDefaultAsync(cancellationToken);
            _ = revision ?? throw new NotFoundException(nameof(Revision), request.ActiveRevisionId);
        }

        return revision;
    }
}
