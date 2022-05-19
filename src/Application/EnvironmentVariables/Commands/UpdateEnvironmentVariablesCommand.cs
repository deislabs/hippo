using Hippo.Application.Common.Interfaces;
using Hippo.Core.Entities;
using MediatR;

namespace Hippo.Application.EnvironmentVariables.Commands;

public class UpdateEnvironmentVariablesCommand : IRequest
{
    public List<EnvironmentVariableRequest> EnvironmentVariables { get; set; } = new List<EnvironmentVariableRequest>();
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

        AddNewEnvironmentVariables(request.EnvironmentVariables);
        UpdateExistingEnvironmentVariables(existingVariables, request.EnvironmentVariables);
        RemoveDeletedEnvironmentVariables(existingVariables, request.EnvironmentVariables);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private List<EnvironmentVariable> GetExistingEnvironmentVariables(Guid channelId)
    {
        return _context.EnvironmentVariables
            .Where(v => v.Id == channelId)
            .ToList();
    }

    private void AddNewEnvironmentVariables(List<EnvironmentVariableRequest> environmentVariables)
    {
        var newVariables = environmentVariables.Where(v => v.Id == null);

        foreach (var environmentVariable in newVariables)
        {
            var entity = new EnvironmentVariable
            {
                Key = environmentVariable.Key,
                Value = environmentVariable.Value,
                ChannelId = environmentVariable.ChannelId
            };

            _context.EnvironmentVariables.Add(entity);
        }
    }

    private void UpdateExistingEnvironmentVariables(List<EnvironmentVariable> existingVariables,
        List<EnvironmentVariableRequest> environmentVariables)
    {
        var environmentVariablesIds = environmentVariables.Select(v => v.Id);

        var toUpdateVariables = existingVariables.Where(v => environmentVariablesIds.Contains(v.Id));

        foreach (var environmentVariable in toUpdateVariables)
        {
            var updatedEnvVar = environmentVariables.First(v => v.Id == environmentVariable.Id);
            environmentVariable.Key = updatedEnvVar.Key;
            environmentVariable.Value = updatedEnvVar.Value;

            _context.EnvironmentVariables.Update(environmentVariable);
        }
    }

    private void RemoveDeletedEnvironmentVariables(List<EnvironmentVariable> existingVariables,
        List<EnvironmentVariableRequest> environmentVariables)
    {
        var environmentVariablesIds = environmentVariables.Select(v => v.Id);

        var toDeleteVariables = existingVariables.Where(v => !environmentVariablesIds.Contains(v.Id));
        _context.EnvironmentVariables.RemoveRange(toDeleteVariables);
    }
}
