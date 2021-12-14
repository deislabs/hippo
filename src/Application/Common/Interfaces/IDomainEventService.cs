using Hippo.Core.Common;

namespace Hippo.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
