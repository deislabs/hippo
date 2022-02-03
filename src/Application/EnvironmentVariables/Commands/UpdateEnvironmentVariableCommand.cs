using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class UpdateEnvironmentVariableCommand : IRequest
{
    public Guid Id { get; set; }

    public string Key { get; set; } = "";

    public string Value { get; set; } = "";
}

public class UpdateEnvironmentVariableCommandHandler : IRequestHandler<UpdateEnvironmentVariableCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEnvironmentVariableCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateEnvironmentVariableCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.EnvironmentVariables
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(EnvironmentVariable), request.Id);
        }

        entity.Key = request.Key;
        entity.Value = request.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
