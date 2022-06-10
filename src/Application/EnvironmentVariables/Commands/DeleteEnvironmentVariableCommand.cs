using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class DeleteEnvironmentVariableCommand : IRequest
{
    [Required]
    public Guid Id { get; set; }
}

public class DeleteEnvironmentVariableCommandHandler : IRequestHandler<DeleteEnvironmentVariableCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteEnvironmentVariableCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteEnvironmentVariableCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.EnvironmentVariables
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(EnvironmentVariable), request.Id);
        }

        entity.AddDomainEvent(new DeletedEvent<EnvironmentVariable>(entity));

        _context.EnvironmentVariables.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
