using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Domains.Commands;

public class CreateDomainCommand : IRequest<Guid>
{
    public string? Name { get; set; }
}

public class CreateDomainCommandHandler : IRequestHandler<CreateDomainCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateDomainCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateDomainCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain
        {
            Name = request.Name
        };

        entity.DomainEvents.Add(new DomainCreatedEvent(entity));

        _context.Domains.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
