using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.Apps.Commands;

public class UpdateAppCommand : IRequest
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? StorageId { get; set; }
}

public class UpdateAppCommandHandler : IRequestHandler<UpdateAppCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAppCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateAppCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Apps
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(App), request.Id);
        }

        entity.Name = request.Name;
        entity.StorageId = request.StorageId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
