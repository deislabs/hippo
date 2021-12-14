using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Channels.Commands;

public class CreateChannelCommand : IRequest<Guid>
{
    public Guid AppId { get; set; }

    public string? Name { get; set; }

    public string? Domain { get; set; }

    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public string? RangeRule { get; set; }

    public Revision? ActiveRevision { get; set; }
}

public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateChannelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        var entity = new Channel
        {
            AppId = request.AppId,
            Name = request.Name,
            Domain = request.Domain,
            RevisionSelectionStrategy = request.RevisionSelectionStrategy,
            RangeRule = request.RangeRule,
            ActiveRevision = request.ActiveRevision,
            PortId = _context.Channels.Count()
        };

        entity.DomainEvents.Add(new ChannelCreatedEvent(entity));

        _context.Channels.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
