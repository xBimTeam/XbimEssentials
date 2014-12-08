#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    XbimException.cs
// (See accompanying copyright.rtf)

#endregion

using System;

namespace Xbim.Common.Exceptions
{
    /// <summary>
    /// Represents an error that occurred within the XBIM application
    /// </summary>
    public class XbimException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XbimException"/> class.
        /// </summary>
        public XbimException()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XbimException(string message)
            : base(message)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner <see cref="Exception"/>.</param>
        public XbimException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
