#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Common
// Filename:    XbimGeometryException.cs
// (See accompanying copyright.rtf)

#endregion
using System;

namespace Xbim.Common.Exceptions
{
    /// <summary>
    /// Represents an error that occurrs while processing geometry for a model file.
    /// </summary>
    public class XbimGeometryException : XbimException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XbimGeometryException"/> class.
        /// </summary>
        public XbimGeometryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimGeometryException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XbimGeometryException(string message)
            : base(message)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimGeometryException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner <see cref="Exception"/>.</param>
        public XbimGeometryException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
