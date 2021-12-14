using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Channels.Commands;

public class UpdateChannelCommand : IRequest
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Domain { get; set; }

    public ChannelRevisionSelectionStrategy RevisionSelectionStrategy { get; set; }

    public string? RangeRule { get; set; }

    public Revision? ActiveRevision { get; set; }
}

public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateChannelCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Channels
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Channel), request.Id);
        }

        if (request.ActiveRevision != null && request.ActiveRevision != entity.ActiveRevision)
        {
            entity.DomainEvents.Add(new ActiveRevisionChangedEvent(entity, request.ActiveRevision));
        }

        entity.Name = request.Name;
        entity.Domain = request.Domain;
        entity.RevisionSelectionStrategy = request.RevisionSelectionStrategy;
        entity.RangeRule = request.RangeRule;
        entity.ActiveRevision = request.ActiveRevision;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
