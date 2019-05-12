using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Common.Exceptions
{
    public class XbimGeometryFaceSetTooLargeException : XbimGeometryException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XbimGeometryFaceSetTooLargeException"/> class.
        /// </summary>
        public XbimGeometryFaceSetTooLargeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimGeometryFaceSetTooLargeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public XbimGeometryFaceSetTooLargeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimGeometryFaceSetTooLargeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner <see cref="Exception"/>.</param>
        public XbimGeometryFaceSetTooLargeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
