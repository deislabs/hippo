using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class UpdateEnvironmentVariablesCommand : IRequest
{
    public List<UpdateEnvironmentVariableDto> EnvironmentVariables { get; set; } = new List<UpdateEnvironmentVariableDto>();
}

public class UpdateEnvironmentVariablesCommandHandler : IRequestHandler<UpdateEnvironmentVariablesCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEnvironmentVariablesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateEnvironmentVariablesCommand request, CancellationToken cancellationToken)
    {
        var existingVariables = GetExistingEnvironmentVariables(request.EnvironmentVariables.First().ChannelId);

        var envVariablesToBeAdded = EnvironmentVariablesToBeAdded(request.EnvironmentVariables);
        var envVariablesToBeUpdated = EnvironmentVariablesToBeUpdated(existingVariables, request.EnvironmentVariables);
        var envVariablesToBeDeleted = EnvironmentVariablesToBeRemoved(existingVariables, request.EnvironmentVariables);

        _context.EnvironmentVariables.AddRange(envVariablesToBeAdded);

        foreach (var environmentVariable in envVariablesToBeUpdated)
        {
            var updatedEnvVar = existingVariables.First(v => v.Id == environmentVariable.Id);
            environmentVariable.Key = updatedEnvVar.Key;
            environmentVariable.Value = updatedEnvVar.Value;

            _context.EnvironmentVariables.Update(environmentVariable);
        }

        _context.EnvironmentVariables.RemoveRange(envVariablesToBeDeleted);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private List<EnvironmentVariable> GetExistingEnvironmentVariables(Guid channelId)
    {
        return _context.EnvironmentVariables
            .Where(v => v.ChannelId == channelId)
            .Include(ev => ev.Channel)
            .Include(ev => ev.Channel.App)
            .ToList();
    }

    private static List<EnvironmentVariable> EnvironmentVariablesToBeAdded(List<UpdateEnvironmentVariableDto> environmentVariables)
    {
        var toBeAdded = new List<EnvironmentVariable>();
        var newVariables = environmentVariables.Where(v => v.Id == null);

        foreach (var environmentVariable in newVariables)
        {
            var entity = new EnvironmentVariable
            {
                Key = environmentVariable.Key,
                Value = environmentVariable.Value,
                ChannelId = environmentVariable.ChannelId
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
