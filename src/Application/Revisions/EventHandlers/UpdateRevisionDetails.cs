using Hippo.Application.Common.Interfaces;
using Hippo.Application.Revisions.Commands;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Revisions.EventHandlers;

public class UpdateRevisionDetails : INotificationHandler<CreatedEvent<Revision>>
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

    public async Task Handle(CreatedEvent<Revision> notification, CancellationToken cancellationToken)
    {
        var revision = notification.Entity;

        _logger.LogInformation($"Hippo Domain Event: {notification.GetType().Name}");

        var revisionDetails = await _bindleService.GetRevisionDetails($"{revision.App.StorageId}/{revision.RevisionNumber}");
        var command = new UpdateRevisionDetailsCommand
        {
            RevisionId = revision.Id,
            RevisionDetails = revisionDetails,
        };

        await _mediator.Send(command, cancellationToken);
    }
}
