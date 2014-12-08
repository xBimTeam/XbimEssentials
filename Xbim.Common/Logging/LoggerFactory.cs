#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    LoggerFactory.cs
// (See accompanying copyright.rtf)

#endregion

using System;
using System.Configuration;
using System.Diagnostics;
using Xbim.Common.Logging.Providers;
using System.Runtime.CompilerServices;

namespace Xbim.Common.Logging
{
	/// <summary>
	/// A class which acts as a Factory for Loggers.
	/// </summary>
	public static class LoggerFactory
	{
		private static ILoggingProvider loggingProvider = null;
		private static bool initialised = false;

		private static ILoggingProvider LoggingProvider
		{
			get
			{
				if (!initialised)
				{
					try
					{
						ResolveLoggingProvider();
						ConfigureLogging();
					}
					catch(Exception ex)
					{
						// An exception here, usually indicates we're not able to resolve a Provider at runtime - Use the default one
						WriteDiagnostics(String.Format("Failed to resolve logging Provider. Using the DefaultProvider. {0}", ex));

						// Default to the standard Provider, because Log4Net may not be available.
						loggingProvider = new DefaultProvider();
						loggingProvider.Configure();
					}
					finally
					{
						initialised = true;
					}
				}

				return loggingProvider;
			}
		}

		private static void WriteDiagnostics(string message)
		{
			// We can't use any ILogger functionality yet as it may not have been configured.
			Trace.TraceError(message);
		}

		#region Configure

		/// <summary>
		/// Resolves the Logging Provider for the current application
		/// </summary>
		private static void ResolveLoggingProvider()
		{
			// TODO: resolve provider through configuration
			// Try Log4net by default. Fail back to built in trace if log4net is not available.
			try
			{
				loggingProvider = new Log4NetProvider();
			}
			catch (Exception)
			{
				loggingProvider = new DefaultProvider();
			}
		}

		/// <summary>
		/// Configures the Logging System.
		/// </summary>
		private static void ConfigureLogging()
		{
			loggingProvider.Configure();
		}


		#endregion Configure

		#region GetLogger

		/// <summary>
		/// Gets an <see cref="ILogger"/> to use for logging, based on the supplied <see cref="Type"/>.
		/// </summary>
		/// <param name="callingType">Type of the caller.</param>
		/// <returns></returns>
		/// <remarks>By supplying the Type it is possible to configure the logging system
		/// to log messages for each type differently. This can allow the user to change the log level,
		/// and the means of output for particular areas of the system.</remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
		public static ILogger GetLogger(Type callingType)
		{
			return LoggingProvider.GetLogger(callingType);
		}

		/// <summary>
		/// Gets a <see cref="ILogger"/> to use for logging, based on the calling methods's Declaring Type.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// By using the caller's Type it is possible to configure the logging system
		/// to log messages from each type differently. This can allow the user to change the log level,
		/// and the means of output for particular areas of the system.
		/// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
		public static ILogger GetLogger()
		{
			return LoggingProvider.GetLogger(GetCallingType());
		}

        [MethodImpl(MethodImplOptions.NoInlining)]
		private static Type GetCallingType()
		{
			System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace();
			// Frame(0) = this method. Frame(1) = caller in this class. Frame(2) = our caller.
			return stack.GetFrame(2).GetMethod().DeclaringType;
		}

        #endregion GetLogger

        /// <summary>
        /// Creates an EventTrace object that captures events from the underlying LoggingProvider using an in memory provider.
        /// </summary>
        /// <remarks>Currently only supported by the Log4Net provider.
        /// The EventTrace class should be Disposed of as soon as possible to avoid excessive memory usage</remarks>
        /// <returns>An EventTrace object</returns>
        public static EventTrace CreateEventTrace()
        {
            return new EventTrace(LoggingProvider);
        }
	}
}
