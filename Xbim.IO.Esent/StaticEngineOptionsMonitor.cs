using Microsoft.Extensions.Options;
using System;


namespace Xbim.IO.Esent
{
    internal sealed class StaticEngineOptionsMonitor : IOptionsMonitor<EsentEngineOptions>
    {
        public StaticEngineOptionsMonitor(EsentEngineOptions currentValue)
        {
            CurrentValue = currentValue; ;
        }

        public IDisposable OnChange(Action<EsentEngineOptions, string> listener) => null;

        public EsentEngineOptions Get(string name) => CurrentValue;

        public EsentEngineOptions CurrentValue { get; }
    }
}
