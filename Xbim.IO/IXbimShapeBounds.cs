using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Geometry;

namespace Xbim.IO
{
    public interface IXbimShapeBounds 
    {
        /// <summary>
        /// The unique label of this shape instance
        /// </summary>
        int InstanceLabel { get;  }
        /// <summary>
        /// The IFC type of the product this instance represents
        /// </summary>
        short IfcTypeId { get; }
        /// <summary>
        /// The label of the IFC Product object that  this instance fully or partly defines
        /// </summary>
        int IfcProductLabel { get; }
        /// <summary>
        /// The style that this shape is presented in when it overrides the shape style
        /// </summary>
        int StyleLabel { get; }
        /// <summary>
        /// The id of the shape geometry  that this is an instance of
        /// </summary>
        int ShapeLabel { get;  }
        /// <summary>
        /// The transformation to be applied to shape to place it in the world coordinates
        /// </summary>
        XbimMatrix3D Transformation { get; }
        /// <summary>
        /// The bounding box of this instance in world coordinates, it has been transformed to the correct location
        /// </summary>
        XbimRect3D BoundingBox { get;  }
        /// <summary>
        /// The cost of this shape in bytes
        /// </summary>
        uint Cost { get; }
        /// <summary>
        /// The number of references to this shape
        /// </summary>
        uint ReferenceCount { get;}
    }
}
