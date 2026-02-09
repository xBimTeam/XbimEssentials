#nullable enable
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Xbim.Essentials.NetCore.Tests
{
    /// <summary>
    /// xUnit ILogger provider that writes to ITestOutputHelper.
    /// </summary>
    public class XunitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;
        private readonly LogLevel _minLevel;

        public XunitLoggerProvider(ITestOutputHelper output, LogLevel minLevel = LogLevel.Debug)
        {
            _output = output;
            _minLevel = minLevel;
        }

        public ILogger CreateLogger(string categoryName) => new XunitLogger(_output, categoryName, _minLevel);

        public void Dispose() { }
    }
}
