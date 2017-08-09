using Microsoft.Extensions.Logging;

namespace Xbim.Common
{
    public static class ApplicationLogging
    {
        private static ILoggerFactory _factory = null;
       

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new LoggerFactory();
                    
                }
                return _factory;
            }
            set { _factory = value; }
        }
        
        public static ILogger CreateLogger<T>() =>  LoggerFactory.CreateLogger<T>();

        
    }
}
