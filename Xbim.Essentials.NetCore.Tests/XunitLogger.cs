#nullable enable
using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xbim.Essentials.NetCore.Tests
{
    /// <summary>
    /// xUnit ILogger implementation that writes to ITestOutputHelper.
    /// </summary>
    public class XunitLogger : ILogger
    {
        private readonly ITestOutputHelper _output;
        private readonly string _categoryName;
        private readonly LogLevel _minLevel;

        public XunitLogger(ITestOutputHelper output, string categoryName, LogLevel minLevel)
        {
            _output = output;
            _categoryName = categoryName;
            _minLevel = minLevel;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            
            try
            {
                var message = formatter(state, exception);
                _output.WriteLine($"[{logLevel}] {_categoryName}: {message}");
                if (exception != null)
                {
                    _output.WriteLine($"  Exception: {exception}");
                }
            }
            catch
            {
                // ITestOutputHelper can throw if test has completed
            }
        }

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }
    }
}
