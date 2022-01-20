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
    public Guid AppId { get; set; }

    public string? Name { get; set; }

    public string? Domain { get; set; }

    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public string? RangeRule { get; set; }

    public Revision? ActiveRevision { get; set; }

    public Certificate? Certificate { get; set; }
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

        var platformDomain = (_config.PlatformDomain != null) ? _config.PlatformDomain : "hippofactory.io";

        var entity = new Channel
        {
            AppId = request.AppId,
            Name = request.Name,
            Domain = (request.Domain != null) ? request.Domain : $"{request.Name}.{app.Name}.{platformDomain}",
            RevisionSelectionStrategy = request.RevisionSelectionStrategy,
            RangeRule = request.RangeRule,
            ActiveRevision = request.ActiveRevision,
            PortId = _context.Channels.Count(),
            Certificate = request.Certificate
        };

        entity.DomainEvents.Add(new ChannelCreatedEvent(entity));

        if (request.Certificate != null)
        {
            entity.DomainEvents.Add(new CertificateBindEvent(request.Certificate, entity));
        }

        _context.Channels.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
