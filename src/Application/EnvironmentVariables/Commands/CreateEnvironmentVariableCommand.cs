using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class CreateEnvironmentVariableCommand : IRequest<Guid>
{
    [Required]
    public string Key { get; set; } = "";

    [Required]
    public string Value { get; set; } = "";

    [Required]
    public Guid ChannelId { get; set; }
}

public class CreateEnvironmentVariableCommandHandler : IRequestHandler<CreateEnvironmentVariableCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateEnvironmentVariableCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateEnvironmentVariableCommand request, CancellationToken cancellationToken)
    {
        var entity = new EnvironmentVariable
        {
            Key = request.Key,
            Value = request.Value,
            ChannelId = request.ChannelId
        };

        entity.DomainEvents.Add(new CreatedEvent<EnvironmentVariable>(entity));

        _context.EnvironmentVariables.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
