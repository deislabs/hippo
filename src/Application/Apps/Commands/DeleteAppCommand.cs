using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.Apps.Commands;

public class DeleteAppCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteAppCommandHandler : IRequestHandler<DeleteAppCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAppCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteAppCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Apps
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(App), request.Id);
        }

        _context.Apps.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
