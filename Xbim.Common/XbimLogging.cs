using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using Xbim.Common.Configuration;

namespace Xbim.Common
{
    /// <summary>
    /// A helper class to centrally manage logging
    /// </summary>
    [Obsolete("Obsoleted. Use Dependency Injection instead")]
    public class XbimLogging
    {

        [Obsolete]
        public XbimLogging() : this(NullLoggerFactory.Instance)
        {

        }

        [Obsolete]
        public XbimLogging(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        private static ILoggerFactory _loggerFactory = null;

        /// <summary>
        /// Gets and sets the <see cref="ILoggerFactory"/> used to construct <see cref="ILogger"/> instances through the XBIM toolkit
        /// </summary>
        /// <remarks>Consumers can provide your own implementation, or add a Provider to the default  
        /// (<see cref="LoggerFactory"/>) implementation</remarks>
        [Obsolete("Use XbimServices.Current.GetLoggerFactory() instead")]
        public static ILoggerFactory LoggerFactory 
        { 
            get
            {
                if (_loggerFactory == null)
                {
                    _loggerFactory = XbimServices.Current.GetLoggerFactory();
                }
                return _loggerFactory;
            }
            set
            {
                _loggerFactory = value;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> Instance
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        [Obsolete("Use XbimServices.Current.CreateLogger")]
        public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
        
        /// <summary>
        /// Creates a new <see cref="ILogger"/><typeparamref name="T"/> Instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("Prefer a DI approach or use XbimServices.Current.CreateLogger<T>", false)]
        public static ILogger<T> CreateLogger<T>() => XbimServices.Current.CreateLogger<T>();
    }
}
