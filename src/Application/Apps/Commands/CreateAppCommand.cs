using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Hippo.Application.Apps.Commands;

public class CreateAppCommand : IRequest<Guid>
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string StorageId { get; set; } = "";
}

public class CreateAppCommandHandler : IRequestHandler<CreateAppCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateAppCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateAppCommand request, CancellationToken cancellationToken)
    {
        var entity = new App
        {
            Name = request.Name,
            StorageId = request.StorageId
        };

        entity.AddDomainEvent(new CreatedEvent<App>(entity));

        _context.Apps.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
