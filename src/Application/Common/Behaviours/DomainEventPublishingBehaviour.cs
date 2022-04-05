using Hippo.Application.Common.Interfaces;
using Hippo.Core.Common;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Common.Behaviours
{
    public class DomainEventPublishingBehaviour<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger _logger;
        private readonly IApplicationDbContext _dbContext;
        private readonly IDomainEventService _domainEventService;

        public DomainEventPublishingBehaviour(
            ILogger<TRequest> logger,
            IApplicationDbContext dbContext,
            IDomainEventService domainEventService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _domainEventService = domainEventService;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            while (true)
            {
                var domainEventEntity = _dbContext.ChangeTracker.Entries<IHasDomainEvent>()
                                                     .Select(x => x.Entity.DomainEvents)
                                                     .SelectMany(x => x)
                                                     .Where(domainEvent => !domainEvent.IsPublished)
                                                     .FirstOrDefault();
                if (domainEventEntity == null) break;

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity);

                _logger.LogInformation("Published event: {Name}", domainEventEntity.GetType().Name);
            }
        }
    }
}
