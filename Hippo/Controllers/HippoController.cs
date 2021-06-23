using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Hippo.Logging;
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

        protected void TraceMessage(string message, [CallerMemberName] string methodName = null)
        {
            _logger.LogTrace($"{methodName}: {message}");
        }

        protected void LogIdMismatch(string objectType, Guid expectedId, Guid formId, [CallerMemberName] string methodName = null)
        {
            _logger.LogWarning($"{methodName}: ${objectType} ID {formId} did not match expected ID {expectedId}");
        }

        protected void TraceMethodEntry([CallerMemberName] string methodName = null)
        {
            TraceMethodEntry(WithArgs(new object[0]), methodName);
        }

        protected void TraceMethodEntry(MethodArgs args, [CallerMemberName] string methodName = null)
        {
            var argsText = args.IsEmpty ? "" : $" with args ({args.Format()})";
            var modelStateText = (ModelState == null || ModelState.IsValid) ? "" : " [model state: invalid]";
            _logger.LogTrace($"{methodName}: entered{argsText}{modelStateText}");
        }

        protected static MethodArgs WithArgs(params object[] args)
        {
            return new MethodArgs(args);
        }

        protected struct MethodArgs
        {
            private readonly object[] _args;

            public MethodArgs(object[] args) => _args = args;

            public bool IsEmpty => _args == null || _args.Length == 0;

            public string Format()
            {
                if (IsEmpty)
                {
                    return "no args";
                }

                return string.Join(", ", _args.Select(FormatOne));
            }

            public static string FormatOne(object arg)
            {
                return arg switch
                {
                    null => "null",
                    ITraceable t => t.FormatTrace(),
                    _ => arg.ToString(),
                };
            }
        }
    }
}
