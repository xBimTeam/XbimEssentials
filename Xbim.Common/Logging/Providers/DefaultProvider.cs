#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    DefaultProvider.cs
// (See accompanying copyright.rtf)

#endregion

using System;

namespace Xbim.Common.Logging.Providers
{
	/// <summary>
	/// Provides basic logging capabilities utilising <see cref="System.Diagnostics.Trace"/> through the <see cref="DefaultLogger"/> Logger.
	/// </summary>
	/// <remarks>A <see cref="System.Diagnostics.TraceListener"/> implementation will be required to 
	/// monitor this log.</remarks>
	internal class DefaultProvider : ILoggingProvider
	{
		#region ILoggingProvider Members

		/// <summary>
		/// Configures the logging environment for first use.
		/// </summary>
		public void Configure()
		{
			// Do nothing
		}

		/// <summary>
		/// Gets the <see cref="ILogger"/> applicable for this <see cref="T:System.Type"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>An <see cref="ILogger"/> for this Type.</returns>
		/// <remarks>The default provider does not make use of the Type.</remarks>
		public ILogger GetLogger(Type type)
		{
			return new DefaultLogger();
		}

        // Null implementation
        public string AttachMemoryLogger()
        {
            return String.Empty;
        }

        public System.Collections.Generic.List<Event> GetEvents(string name)
        {
            return new System.Collections.Generic.List<Event>() ;
        }

        public void DetatchMemoryLogger(string name)
        {
            // Do nothing
        }
        #endregion
    }
}
