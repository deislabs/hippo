using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Hippo.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hippo.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected void LogIfNotFound<T>(T entity, object entityId, [CallerMemberName] string methodName = null)
        {
            if (entity == null)
            {
                var entityType = typeof(T).Name.ToUpperInvariant();
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
            TraceMethodEntry(WithArgs(Array.Empty<object>()), methodName);
        }

        protected void TraceMethodEntry(MethodArgs args, [CallerMemberName] string methodName = null)
        {
            var argsText = args.IsEmpty ? "" : $" with args ({args.Format()})";
            var modelStateText = (ModelState == null || ModelState.IsValid) ? "" : " [model state: invalid]";
            _logger.LogTrace($"{methodName}: entered{argsText}{modelStateText}");
        }

        protected CreatedResult Created(object value) => new("", value);

        protected static MethodArgs WithArgs(params object[] args)
        {
            return new MethodArgs(args);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not needed, not used externally or for equality")]
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
