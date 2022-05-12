using Hippo.Application.Channels.Commands;
using Hippo.Application.Common.Config;
using Hippo.Application.Common.Models;
using Hippo.Core.Entities;
using Hippo.Core.Enums;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Apps.EventHandlers;

public class CreateInitialChannel : INotificationHandler<DomainEventNotification<CreatedEvent<App>>>
{
    private readonly ILogger<AppCreatedEventHandler> _logger;

    private readonly IMediator _mediator;

    private readonly HippoConfig _config;

    public CreateInitialChannel(ILogger<AppCreatedEventHandler> logger, IMediator mediator, HippoConfig config)
    {
        _logger = logger;
        _mediator = mediator;
        _config = config;
    }

    public async Task Handle(DomainEventNotification<CreatedEvent<App>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var app = domainEvent.Entity;

        var command = new CreateChannelCommand
        {
            AppId = app.Id,
            Name = "Production",
            RevisionSelectionStrategy = ChannelRevisionSelectionStrategy.UseRangeRule,
            RangeRule = "*",
            Domain = $"{app.Name}.{_config.PlatformDomain}".Replace('_', '-').ToLower(),
        };

        await _mediator.Send(command, cancellationToken);

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);
    }
}
