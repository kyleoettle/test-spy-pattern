using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Test.Spy.Pattern.UnitTests.Spies
{
    public class SpyLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return A.Fake<IDisposable>();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        internal Action<LogLevel, EventId, object, Exception?> _logInvoked;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (_logInvoked == null)
                throw new NotImplementedException(nameof(_logInvoked));

            _logInvoked(logLevel, eventId, state, exception);
        }
    }
}
