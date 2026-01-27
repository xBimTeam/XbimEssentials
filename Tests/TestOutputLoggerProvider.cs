using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Xbim.Essentials.Tests
{
    internal class TestOutputLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;
        public TestOutputLoggerProvider(ITestOutputHelper output) => _output = output;
        public ILogger CreateLogger(string categoryName) => new TestOutputLogger(_output, categoryName);
        public void Dispose() { }
    }
    internal class TestOutputLogger : ILogger
    {
        private readonly ITestOutputHelper _output;
        private readonly string _category;
        public TestOutputLogger(ITestOutputHelper output, string category)
        {
            _output = output;
            _category = category;
        }
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter)
        {
            _output.WriteLine($"[{logLevel}] {_category}: {formatter(state, exception)}");
        }
    }
}
