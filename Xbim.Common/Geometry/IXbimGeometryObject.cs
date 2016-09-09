using System;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// Abstract class for all Xbim Geometry objects
    /// </summary>
    public interface IXbimGeometryObject : IDisposable
    {
        XbimGeometryObjectType GeometryType { get; }
        bool IsValid { get; }
        /// <summary>
        /// True if the geometry object is a collection
        /// </summary>
        bool IsSet { get; }
        XbimRect3D BoundingBox { get; }
        /// <summary>
        /// Returns a copy of the current object transformed by matrix3D, it is gauranteed to return the same type as "this"
        /// </summary>
        /// <param name="matrix3D"></param>
        /// <returns></returns>
        IXbimGeometryObject Transform(XbimMatrix3D matrix3D);
        /// <summary>
        /// Returns a new version of the object transformed but does not perform a deepcopy, changes to this  will be reflected in the copy and vice versa
        /// </summary>
        /// <param name="matrix3D"></param>
        /// <returns></returns>
        IXbimGeometryObject TransformShallow(XbimMatrix3D matrix3D);
        /// <summary>
        /// Gets or sets an arbitrary object value that can be used to store custom information about this element
        /// </summary>
        object Tag { get; set; }

        string ToBrep { get; }
       
    }
}
