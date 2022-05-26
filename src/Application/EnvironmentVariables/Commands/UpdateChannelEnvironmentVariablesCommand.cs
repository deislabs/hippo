using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class UpdateChannelEnvironmentVariablesCommand : IRequest
{
    public Guid ChannelId { get; set; }
    public List<UpdateEnvironmentVariableDto> EnvironmentVariables { get; set; } = new List<UpdateEnvironmentVariableDto>();
}

public class UpdateEnvironmentVariablesCommandHandler : IRequestHandler<UpdateChannelEnvironmentVariablesCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEnvironmentVariablesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateChannelEnvironmentVariablesCommand request, CancellationToken cancellationToken)
    {
        var channel = _context.Channels
            .Include(c => c.App)
            .FirstOrDefault(c => c.Id == request.ChannelId);

        var existingVariables = GetExistingEnvironmentVariables(request.ChannelId);

        var envVariablesToBeAdded = EnvironmentVariablesToBeAdded(request.EnvironmentVariables, channel);
        var envVariablesToBeUpdated = EnvironmentVariablesToBeUpdated(existingVariables, request.EnvironmentVariables);
        var envVariablesToBeDeleted = EnvironmentVariablesToBeRemoved(existingVariables, request.EnvironmentVariables);

        _context.EnvironmentVariables.AddRange(envVariablesToBeAdded);

        if (existingVariables.Count > 0)
        {
            foreach (var environmentVariable in request.EnvironmentVariables)
            {
                var updatedEnvVar = existingVariables.FirstOrDefault(v => v.Id == environmentVariable.Id);
                
                if (updatedEnvVar == null)
                {
                    continue;
                }
                updatedEnvVar.Key = environmentVariable.Key;
                updatedEnvVar.Value = environmentVariable.Value;
            }
        }

        _context.EnvironmentVariables.RemoveRange(envVariablesToBeDeleted);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private List<EnvironmentVariable> GetExistingEnvironmentVariables(Guid channelId)
    {
        return _context.EnvironmentVariables
            .Where(v => v.ChannelId == channelId)
            .ToList();
    }

    private static List<EnvironmentVariable> EnvironmentVariablesToBeAdded(List<UpdateEnvironmentVariableDto> environmentVariables, Channel channel)
    {
        var toBeAdded = new List<EnvironmentVariable>();
        var newVariables = environmentVariables.Where(v => v.Id == null);

        foreach (var environmentVariable in newVariables)
        {
            var entity = new EnvironmentVariable
            {
                Key = environmentVariable.Key,
                Value = environmentVariable.Value,
                Channel = channel
            };

            toBeAdded.Add(entity);
        }

        return toBeAdded;
    }

    private static List<EnvironmentVariable> EnvironmentVariablesToBeUpdated(List<EnvironmentVariable> existingVariables,
        List<UpdateEnvironmentVariableDto> environmentVariables)
    {
        var environmentVariablesIds = environmentVariables.Select(v => v.Id);

        return existingVariables.Where(v => environmentVariablesIds.Contains(v.Id)).ToList();
    }

    private static List<EnvironmentVariable> EnvironmentVariablesToBeRemoved(List<EnvironmentVariable> existingVariables,
        List<UpdateEnvironmentVariableDto> environmentVariables)
    {
        var environmentVariablesIds = environmentVariables.Select(v => v.Id);

        return existingVariables.Where(v => !environmentVariablesIds.Contains(v.Id)).ToList();
    }
}
