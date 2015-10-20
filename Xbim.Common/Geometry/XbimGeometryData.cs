using System;
using System.Collections.Generic;
using System.Linq;
using XbimGeometry.Interfaces;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// Conpares the shape data of two geometry objects to see if they are the same
    /// </summary>
    public class XbimShapeEqualityComparer : IEqualityComparer<XbimGeometryData>
    {
        public bool Equals(XbimGeometryData x, XbimGeometryData y)
        {
            return x.ShapeData.SequenceEqual(y.ShapeData);
        }

        public int GetHashCode(XbimGeometryData obj)
        {
            return obj.ShapeData.Length;
        }
    }

    public class XbimGeometryData
    {
        readonly public int GeometryLabel;
        readonly public int IfcProductLabel;
        readonly public XbimGeometryType GeometryType;
        readonly public byte[] ShapeData;
        readonly public byte[] DataArray2;
        readonly public int GeometryHash;
        readonly public short IfcTypeId;
        readonly public int StyleLabel;
        readonly public int Counter;
       
     
        public XbimGeometryData(int geometrylabel, int productLabel, XbimGeometryType geomType, short ifcTypeId, byte[] shape, byte[] dataArray2, int geometryHash, int styleLabel, int counter)
        {
            GeometryLabel = geometrylabel;
            GeometryType = geomType;
            IfcTypeId = ifcTypeId;
            ShapeData = shape;
            IfcProductLabel = productLabel;
            GeometryHash = geometryHash;
            StyleLabel = styleLabel;
            DataArray2 = dataArray2;
            Counter = counter;
        }

        // TODO: when marking methods obsolete we should provide guidance for removing them from user's codebases
        // in this case the suggestion will probably be to re-mesh, but no clear API is available for that; we have to hava a policy in place

        /// <summary>
        /// Transforms the shape data of the geometry by the matrix
        /// NB This is a deprecated method and will be removed for the latest geometry support and is only used in first geometry implementation
        /// </summary>
        /// <param name="matrix"></param>
        [Obsolete ("This method should not be used and is marked for deletion",false)]
        public XbimGeometryData TransformBy(XbimMatrix3D matrix)
        {
            XbimMatrix3D t =  XbimMatrix3D.FromArray(DataArray2);
            t = XbimMatrix3D.Multiply(t, matrix);
            return new XbimGeometryData(GeometryLabel, IfcProductLabel, GeometryType, IfcTypeId, ShapeData, t.ToArray(), GeometryHash, StyleLabel, Counter);
        }

       
        /// <summary>
        /// The constructs an XbimGeoemtryData object, the geometry hash is calculated from the array of shape data
        /// </summary>
        /// <param name="geometrylabel"></param>
        /// <param name="productLabel"></param>
        /// <param name="geomType"></param>
        /// <param name="ifcTypeId"></param>
        /// <param name="shape"></param>
        /// <param name="transform"></param>
        /// <param name="styleLabel"></param>
        public XbimGeometryData(int geometrylabel, int productLabel, XbimGeometryType geomType, short ifcTypeId, byte[] shape, byte[] transform, int styleLabel)
        {
            GeometryLabel = geometrylabel;
            GeometryType = geomType;
            IfcTypeId = ifcTypeId;
            ShapeData = shape;
            DataArray2  = transform;
            IfcProductLabel = productLabel;
            GeometryHash = GenerateGeometryHash(ShapeData);
            StyleLabel = styleLabel;
        }

        
        
        /// <summary>
        /// Returns true if the two geometries have identical shape data
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool IsGeometryEqual(XbimGeometryData to)
        {

            return ShapeData.SequenceEqual(to.ShapeData);
        }


        /// <summary>
        /// Generates a FNV hash for any array of bytes
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        static public int GenerateGeometryHash(byte[] array)
        {
            unchecked
            {
                const int p = 16777619;
                var hash = array.Aggregate((int) 2166136261, (current, t) => (current ^ t)*p);

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
    }
}
