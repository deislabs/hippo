using Hippo.Application.Common.Interfaces;
using Hippo.Application.Common.Models;
using Hippo.Core.Entities;
using Hippo.Core.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hippo.Application.Revisions.EventHandlers;

public class UpdateRevisionDetails : INotificationHandler<DomainEventNotification<CreatedEvent<Revision>>>
{
    private readonly ILogger<UpdateRevisionDetails> _logger;

    private readonly IApplicationDbContext _context;

    private readonly IBindleService _bindleService;

    public UpdateRevisionDetails(ILogger<UpdateRevisionDetails> logger,
        IApplicationDbContext context,
        IBindleService bindleService)
    {
        _logger = logger;
        _context = context;
        _bindleService = bindleService;
    }

    public async Task Handle(DomainEventNotification<CreatedEvent<Revision>> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var revision = notification.DomainEvent.Entity;

        _logger.LogInformation("Hippo Domain Event: {DomainEvent}", domainEvent.GetType().Name);

        var revisionDetails = await _bindleService.GetRevisionDetails($"{revision.App.StorageId}/{revision.RevisionNumber}");
        if (revisionDetails == null || revisionDetails.SpinToml == null)
        {
            return;
        }

        revision.Description = revisionDetails.Description;
        var newComponents = revisionDetails.SpinToml.Component
            .Select(c => new RevisionComponent
        {
            Source = c.Source,
            Name = c.Id,
            Route = c.Trigger?.Route ?? "/",
            Revision = revision,
        });
        _context.RevisionComponents.AddRange(newComponents);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
