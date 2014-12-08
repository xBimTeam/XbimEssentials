#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    ILoggingProvider.cs
// (See accompanying copyright.rtf)

#endregion

using System;
using System.Collections.Generic;

namespace Xbim.Common.Logging.Providers
{
	/// <summary>
	/// Defines the interface for a provider of logging services.
	/// </summary>
	interface ILoggingProvider
	{
		/// <summary>
		/// Configures the logging environment for first use.
		/// </summary>
		void Configure();

		/// <summary>
		/// Gets the <see cref="ILogger"/> applicable for this <see cref="T:System.Type"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <remarks>Logging consumers provider a Type to this call so that the Logging Provider
		/// can customise the logger dynamically for the Type. More advanced logging systems, such 
		/// as log4Net can use this to provide different logging levels and outputs for different
		/// parts of the application.</remarks>
		/// <returns>An <see cref="ILogger"/> for this Type.</returns>
		ILogger GetLogger(Type type);

        String AttachMemoryLogger();

        List<Event> GetEvents(String name);

        void DetatchMemoryLogger(String name);

	}
}
