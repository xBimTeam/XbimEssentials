using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace Xbim.Common
{
    /// <summary>
    /// A helper class to centrally manage logging
    /// </summary>
    [Obsolete("Obsoleted. Use Dependency Injection instead")]
    public class XbimLogging
    {

        public XbimLogging() : this(NullLoggerFactory.Instance)
        {

        }

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
        public static ILoggerFactory LoggerFactory 
        { 
            get
            {
                if (_loggerFactory == null)
                {
                    _loggerFactory = NullLoggerFactory.Instance;
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
        public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
        /// <summary>
        /// Creates a new <see cref="ILogger"/><typeparamref name="T"/> Instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("Prefer DI", false)]
        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}
