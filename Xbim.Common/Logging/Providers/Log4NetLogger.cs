#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    Log4NetLogger.cs
// (See accompanying copyright.rtf)

#endregion

using System;
using System.Globalization;
using log4net.Core;
using log4net.Util;

namespace Xbim.Common.Logging.Providers
{
	/// <summary>
	/// A simple wrapper class for the Log4Net logging framework.
	/// </summary>
	/// <remarks>Decouples consumers from the underlying Log4Net assemblies, 
	/// while exposing its functionality.</remarks>
	internal class Log4NetLogger : ILogger
	{
		private readonly log4net.ILog _logger;

		private const string Logging_UnhandledException = "Unhandled Exception";

		/// <summary>
		/// Type used to tell the base log4Net Logger which is the boundary in the stack frame 
		/// indicating the logger entry point.
		/// </summary>
		/// <remarks>Means we get accurate Class/Method info in the logs.</remarks>
		private readonly static Type ThisDeclaringType = typeof(Log4NetLogger); 

		/// <summary>
		/// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
		/// </summary>
		/// <param name="type">The calling type.</param>
		/// <remarks>By building a logger for each type we can take advantage of log4Net's 
		/// hierarchical configuration system.</remarks>
		public Log4NetLogger(Type type)
		{
			_logger = log4net.LogManager.GetLogger(type);
		}

		#region ILogger Members

		/// <summary>
		/// Logs the specified message with the <c>DEBUG</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Debug(object message)
		{
			// Rather than delegate straight to _logger.Debug, we use the underlying ILog.Logger.Log mechanism, 
			// which lets us specify the Type of this facade (which is used in determining the caller).
			// Without this, the call stack would indicate all logs originating in _this_ class, not the
			// caller.
			_logger.Logger.Log(ThisDeclaringType, Level.Debug, message, null);
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>DEBUG</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Debug(object message, Exception exception)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Debug, message, exception);
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
				_logger.Logger.Log(ThisDeclaringType, Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
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
			get { return _logger.IsDebugEnabled; }
		}

		/// <summary>
		/// Logs the specified message with the <c>INFO</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Info(object message)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Info, message, null);
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>INFO</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Info(object message, Exception exception)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Info, message, exception);
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
				_logger.Logger.Log(ThisDeclaringType, Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
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
			get { return _logger.IsInfoEnabled; }
		}


		/// <summary>
		/// Logs the specified message with the <c>WARN</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Warn(object message)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Warn, message, null);
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>WARN</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Warn(object message, Exception exception)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Warn, message, exception);
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
				_logger.Logger.Log(ThisDeclaringType, Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
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
			get { return _logger.IsWarnEnabled; }
		}

		/// <summary>
		/// Logs the specified message with the <c>ERROR</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Error(object message)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Error, message, null);
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>ERROR</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Error(object message, Exception exception)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Error, message, exception);
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
				_logger.Logger.Log(ThisDeclaringType, Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
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
			get { return _logger.IsErrorEnabled; }
		}

		/// <summary>
		/// Logs the specified message with the <c>FATAL</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		public void Fatal(object message)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Fatal, message, null);
		}

		/// <summary>
		/// Logs the specified message and exception with the <c>FATAL</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		public void Fatal(object message, Exception exception)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Fatal, message, exception);
		}

		/// <summary>
		/// Logs a formatted message string with the <c>FATAL</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		public void FatalFormat(string format, params object[] args)
		{
			if (IsFatalEnabled)
			{
				_logger.Logger.Log(ThisDeclaringType, Level.Fatal, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Fatal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Fatal enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsFatalEnabled
		{
			get { return _logger.IsFatalEnabled; }
		}


		/// <summary>
		/// Logs the unhandled exception with Fatal level
		/// </summary>
		/// <param name="exception">The exception.</param>
		public void UnhandledException(Exception exception)
		{
			_logger.Logger.Log(ThisDeclaringType, Level.Fatal, Logging_UnhandledException, exception);
		}

		#endregion
	}
}
