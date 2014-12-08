using System;
using System.Collections.Generic;
using Xbim.Common.Logging.Providers;

namespace Xbim.Common.Logging
{
    /// <summary>
    /// Logging helper class that enables an application to access the logger data as an in-memory collection of Events.
    /// </summary>
    public class EventTrace : IDisposable
    {

        ILoggingProvider _provider;
        private string _loggerName;
        private bool _disposed;

        internal EventTrace(ILoggingProvider provider)
        {
            _provider = provider;

            // Set up a memory logger if the provider allows
            _loggerName = provider.AttachMemoryLogger();
            
            _disposed = false;
        }

        public IList<Event> Events
        {
            get
            {
                return _provider.GetEvents(_loggerName);
            }
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _provider.DetatchMemoryLogger(_loggerName);
                }

                _provider = null;
                _disposed = true;
            }
        } 
        #endregion

    }
}
