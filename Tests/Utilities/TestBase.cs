using Microsoft.Extensions.Logging;
using Xbim.Common.Configuration;

namespace Xbim.Essentials.Tests
{
    public abstract class TestBase
    {
        public TestBase()
        {

        }

        protected static ILoggerFactory LoggerFactory { get; } = XbimServices.Current.GetLoggerFactory();


        
    }
}
