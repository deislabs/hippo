using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Application.Revisions.Commands;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Revisions.EventHandlers;

public class UpdateRevisionDetails : INotificationHandler<DomainEventNotification<CreatedEvent<Revision>>>
{
    private readonly ILogger<UpdateRevisionDetails> _logger;

    private readonly IBindleService _bindleService;

    private readonly IMediator _mediator;

    public UpdateRevisionDetails(ILogger<UpdateRevisionDetails> logger,
        IBindleService bindleService,
        IMediator mediator)
    {
        _logger = logger;
        _bindleService = bindleService;
        _mediator = mediator;
    }

    public async Task Handle(DomainEventNotification<CreatedEvent<Revision>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var revision = notification.DomainEvent.Entity;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        var revisionDetails = await _bindleService.GetRevisionDetails($"{revision.App.StorageId}/{revision.RevisionNumber}");
        var command = new UpdateRevisionDetailsCommand
        {
            RevisionId = revision.Id,
            RevisionDetails = revisionDetails,
        };

        await _mediator.Send(command, cancellationToken);
    }
}
