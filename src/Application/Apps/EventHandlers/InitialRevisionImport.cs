using Hippo.Application.Revisions.Commands;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Apps.EventHandlers;

public class InitialRevisionImport : INotificationHandler<CreatedEvent<App>>
{
    private readonly ILogger<InitialRevisionImport> _logger;
    private readonly IMediator _mediator;

    public InitialRevisionImport(ILogger<InitialRevisionImport> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Handle(CreatedEvent<App> notification, CancellationToken cancellationToken)
    {
        var app = notification.Entity;

        var command = new ImportRevisionsCommand(app);

        await _mediator.Send(command, cancellationToken);

        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");
    }
}
