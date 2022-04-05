using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Config;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
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

        if (app == null)
        {
            throw new NotFoundException(nameof(App), request.AppId);
        }

        var entity = new Channel
        {
            AppId = request.AppId,
            Name = request.Name,
            Domain = (request.Domain != null) ? request.Domain : $"{request.Name}.{app.Name}.{_config.PlatformDomain}",
            RevisionSelectionStrategy = request.RevisionSelectionStrategy,
            RangeRule = request.RangeRule,
            ActiveRevisionId = request.ActiveRevisionId,
            PortId = _context.Channels.Count(),
            CertificateId = request.CertificateId
        };

        if (entity.RevisionSelectionStrategy == ChannelRevisionSelectionStrategy.UseRangeRule && entity.RangeRule == null)
        {
            entity.RangeRule = "*";
        }

        entity.DomainEvents.Add(new CreatedEvent<Channel>(entity));

        _context.Channels.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
