using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Revisions.Commands;

public class CreateRevisionCommand : IRequest<Guid>
{
    public Guid AppId { get; set; }

    public string? RevisionNumber { get; set; }
}

public class CreateRevisionCommandHandler : IRequestHandler<CreateRevisionCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateRevisionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateRevisionCommand request, CancellationToken cancellationToken)
    {
        var entity = new Revision
        {
            AppId = request.AppId,
            RevisionNumber = request.RevisionNumber
        };

        entity.DomainEvents.Add(new RevisionCreatedEvent(entity));

        _context.Revisions.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
