#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    XbimParserException.cs
// (See accompanying copyright.rtf)

#endregion
using System;

namespace Xbim.Common.Exceptions
{

    /// <summary>
    /// Represents an error that occurrs while parsing a model file.
    /// </summary>
    public class XbimParserException : XbimException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XbimParserException"/> class.
        /// </summary>
        public XbimParserException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimParserException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XbimParserException(string message)
            : base(message)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimParserException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner <see cref="Exception"/>.</param>
        public XbimParserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
