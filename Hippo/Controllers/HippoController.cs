using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hippo.Controllers
{
    public abstract class HippoController : Controller
    {
        protected readonly ILogger _logger;

        protected HippoController(ILogger logger)
        {
            _logger = logger;
        }

        protected void LogIfNotFound<T>(T entity, object entityId, [CallerMemberName] string methodName = null)
        {
            if (entity == null)
            {
                var entityType = typeof(T).Name.ToLowerInvariant();
                _logger.LogWarning($"{methodName}: {entityType} {entityId} not found");
            }
        }
    }
}