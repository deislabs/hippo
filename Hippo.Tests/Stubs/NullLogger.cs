using System;
using Microsoft.Extensions.Logging;

namespace Hippo.Tests.Stubs
{
    internal class NullLogger<T> : ILogger<T>
    {
        private readonly bool _log;

        public NullLogger(bool log = false)
        {
            _log = log;
        }
        public IDisposable BeginScope<TState>(TState state) =>
            throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_log)
            {
                Console.WriteLine(state);
                Console.WriteLine(exception);
            }
        }
    }
}
