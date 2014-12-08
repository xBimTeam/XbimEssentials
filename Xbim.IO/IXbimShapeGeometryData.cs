using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.IO
{
    public interface IXbimShapeGeometryData
    {
        /// <summary>
        /// The unique label of this shape instance
        /// </summary>
        int ShapeLabel{get;set;}
        /// <summary>
        /// The label of the IFC object that defines this shape
        /// </summary>
        int IfcShapeLabel{get;set;}
        /// <summary>
        ///  Hash of the shape Geometry, based on the IFC representation, this is not unique
        /// </summary>
        int GeometryHash{get;set;}
        /// <summary>
        /// The cost of this shape in bytes
        /// </summary>
        int Cost { get; }
        /// <summary>
        /// The number of references to this shape
        /// </summary>
        int ReferenceCount{get;set;}
        /// <summary>
        /// The level of detail or development that the shape is suited for
        /// </summary>
        byte LOD{get;set;}
        /// <summary>
        /// The format in which the shape data is represented, i.e. triangular mesh, polygon, opencascade
        /// </summary>
        byte Format{get;set;}
        /// <summary>
        /// The bounding box of this instance in world coordinates, it has been transformed to the correct location
        /// </summary>
        byte[] BoundingBox { get; set; }
        /// <summary>
        /// The geometry data defining the shape in zip compression
        /// </summary>
        byte[] ShapeDataCompressed{get;set;}
        /// <summary>
        /// The geometry data defining the shape in  uncompressed format
        /// </summary>
        byte[] ShapeData { get; set; }
    }
}
