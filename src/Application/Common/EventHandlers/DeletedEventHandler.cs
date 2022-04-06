// TODO: uncomment once .NET 7 has been released
//
// https://github.com/dotnet/runtime/issues/65145

// using Hippo.Application.Common.Models;
// using Hippo.Core.Common;
// using Hippo.Core.Events;
// using MediatR;
// using Microsoft.Extensions.Logging;

// namespace Hippo.Application.Common.EventHandlers;

// public class DeletedEventHandler<T> : INotificationHandler<DomainEventNotification<DeletedEvent<T>>> where T : IHasDomainEvent
// {
//     private readonly ILogger<DeletedEventHandler<T>> _logger;

//     public DeletedEventHandler(ILogger<DeletedEventHandler<T>> logger)
//     {
//         _logger = logger;
//     }

//     public Task Handle(DomainEventNotification<DeletedEvent<T>> notification, CancellationToken cancellationToken)
//     {
//         var domainEvent = notification.DomainEvent;

//         _logger.LogInformation($"Hippo Domain Event: {domainEvent.GetType().Name}");

//         return Task.CompletedTask;
//     }
// }
