#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    ILogger.cs
// (See accompanying copyright.rtf)

#endregion

using System;

namespace Xbim.Common.Logging
{
	/// <summary>
	/// Defines the interface of a generic logging system.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Logs the specified message with the <c>DEBUG</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Debug(object message);

		/// <summary>
		/// Logs the specified message and exception with the <c>DEBUG</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		void Debug(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <c>DEBUG</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		void DebugFormat(string format, params object[] args);

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Debug.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Debug enabled; otherwise, <c>false</c>.
		/// </value>
		bool IsDebugEnabled { get; }

		/// <summary>
		/// Logs the specified message with the <c>INFO</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Info(object message);

		/// <summary>
		/// Logs the specified message and exception with the <c>INFO</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		void Info(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <c>INFO</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		void InfoFormat(string format, params object[] args);

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Info.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Info enabled; otherwise, <c>false</c>.
		/// </value>
		bool IsInfoEnabled { get; }

		/// <summary>
		/// Logs the specified message with the <c>WARN</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Warn(object message);

		/// <summary>
		/// Logs the specified message and exception with the <c>WARN</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		void Warn(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <c>WARN</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		void WarnFormat(string format, params object[] args);

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Warn.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Warn enabled; otherwise, <c>false</c>.
		/// </value>
		bool IsWarnEnabled { get; }

		/// <summary>
		/// Logs the specified message with the <c>ERROR</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Error(object message);

		/// <summary>
		/// Logs the specified message and exception with the <c>ERROR</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		void Error(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <c>ERROR</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		void ErrorFormat(string format, params object[] args);

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Error.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Error enabled; otherwise, <c>false</c>.
		/// </value>
		bool IsErrorEnabled { get; }

		/// <summary>
		/// Logs the specified message with the <c>FATAL</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		void Fatal(object message);

		/// <summary>
		/// Logs the specified message and exception with the <c>FATAL</c> level
		/// </summary>
		/// <param name="message">The message object to log.</param>
		/// <param name="exception">The exception.</param>
		void Fatal(object message, Exception exception);

		/// <summary>
		/// Logs a formatted message string with the <c>FATAL</c> level.
		/// </summary>
		/// <param name="format">A String containing zero or more format items</param>
		/// <param name="args">An Object array containing zero or more objects to format</param>
		void FatalFormat(string format, params object[] args);

		/// <summary>
		/// Gets a value indicating whether this log is enabled for Fatal.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this log is Fatal enabled; otherwise, <c>false</c>.
		/// </value>
		bool IsFatalEnabled { get; }

		/// <summary>
		/// Logs the unhandled exception with Fatal level
		/// </summary>
		/// <param name="exception">The exception.</param>
		void UnhandledException(Exception exception);
	}
}
