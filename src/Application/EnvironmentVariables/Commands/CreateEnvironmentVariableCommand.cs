using System.ComponentModel.DataAnnotations;
using Hippo.Application.Common.Exceptions;
using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        var channel = await _context.Channels
            .Where(a => a.Id == request.ChannelId)
            .Include(c => c.App)
            .SingleOrDefaultAsync(cancellationToken);
        _ = channel ?? throw new NotFoundException(nameof(Channel), request.ChannelId);

        var entity = new EnvironmentVariable
        {
            Key = request.Key,
            Value = request.Value,
            ChannelId = request.ChannelId,
            Channel = channel,
        };

        _context.EnvironmentVariables.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
