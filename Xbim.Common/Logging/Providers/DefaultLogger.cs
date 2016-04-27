#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    DefaultLogger.cs
// (See accompanying copyright.rtf)

#endregion

using System;
using System.Diagnostics;
using System.Globalization;

namespace Xbim.Common.Logging.Providers
{
	/// <summary>
	/// A Default logging system based on <see cref="System.Diagnostics.Trace"/>
	/// </summary>
	internal class DefaultLogger : ILogger
	{
		// TODO: set from config
		static readonly TraceSwitch Trace = new TraceSwitch("TraceLog", "Global Tracelog");

	    #region ILogger Members

		/// <summary>
		/// Logs the specified message with the <c>DEBUG</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Debug(object message)
		{
			if (IsDebugEnabled)
				System.Diagnostics.Trace.WriteLine(message);
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>DEBUG</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Debug(object message, Exception exception)
		{
			if (IsDebugEnabled)
			{
				System.Diagnostics.Trace.WriteLine(message);
				WriteException(TraceLevel.Verbose, exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>DEBUG</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		public void DebugFormat(string format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				System.Diagnostics.Trace.WriteLine(String.Format(CultureInfo.InvariantCulture, format, args));
			}
		}

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Debug.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Debug enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsDebugEnabled
		{
			get { return Trace.TraceVerbose; }
		}

		/// <summary>
		/// Logs the specified message with the <c>INFO</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Info(object message)
		{
			if (IsInfoEnabled)
				System.Diagnostics.Trace.WriteLine(TraceLevel.Info, message.ToString());
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>INFO</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Info(object message, Exception exception)
		{
			if (IsInfoEnabled)
			{
				System.Diagnostics.Trace.TraceInformation(message.ToString());
				WriteException(TraceLevel.Info, exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>INFO</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		public void InfoFormat(string format, params object[] args)
		{
			if (IsInfoEnabled)
			{
				System.Diagnostics.Trace.TraceInformation(String.Format(CultureInfo.InvariantCulture, format, args));
			}
		}

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Info.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Info enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsInfoEnabled
		{
			get { return Trace.TraceInfo; }
		}


		/// <summary>
		/// Logs the specified message with the <c>WARN</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Warn(object message)
		{
			if (IsWarnEnabled)
				System.Diagnostics.Trace.TraceWarning(message.ToString());
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>WARN</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Warn(object message, Exception exception)
		{
			if (IsWarnEnabled)
			{
				System.Diagnostics.Trace.TraceWarning(message.ToString());
				WriteException(TraceLevel.Warning, exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>WARN</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		public void WarnFormat(string format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				System.Diagnostics.Trace.TraceWarning(String.Format(CultureInfo.InvariantCulture, format, args));
			}
		}

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Warn.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Warn enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsWarnEnabled
		{
			get { return Trace.TraceWarning; }
		}

		/// <summary>
		/// Logs the specified message with the <c>ERROR</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Error(object message)
		{
			if (IsErrorEnabled)
				System.Diagnostics.Trace.TraceError(message.ToString());
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>ERROR</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Error(object message, Exception exception)
		{
			if (IsErrorEnabled)
			{
				System.Diagnostics.Trace.TraceError(message.ToString());
				WriteException(TraceLevel.Error, exception);
			}
		}

		/// <summary>
		/// Logs a formatted message string with the <c>ERROR</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		public void ErrorFormat(string format, params object[] args)
		{
			if (IsErrorEnabled)
			{
				System.Diagnostics.Trace.TraceError(String.Format(CultureInfo.InvariantCulture, format, args));
			}
		}

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Error.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Error enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsErrorEnabled
		{
			get { return Trace.TraceError; }
		}

		/// <summary>
		/// Logs the specified message with the <c>FATAL</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Fatal(object message)
		{
			Error(message);
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>FATAL</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Fatal(object message, Exception exception)
		{
			Error(message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FATAL</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		public void FatalFormat(string format, params object[] args)
		{
			ErrorFormat(format, args);
		}

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Fatal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Fatal enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsFatalEnabled
		{
			get { return IsErrorEnabled; }
		}


		/// <summary>
		/// Logs the unhandled exception with Fatal level
		/// </summary>
		/// <param name="exception">The exception.</param>
		public void UnhandledException(Exception exception)
		{
			WriteException(TraceLevel.Error, exception);
		}

		#endregion

		private static void WriteException(TraceLevel traceLevel, Exception exception)
		{
			System.Diagnostics.Trace.TraceError(exception.ToString());
		}
	}
}
