using System.Collections.Generic;
using Xbim.Common.Exceptions;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.TopologyResource;


namespace Xbim.Ifc2x3.Extensions
{
    public struct RepresentationItemComparer : IEqualityComparer<IfcRepresentationItem>
    {

        public bool Equals(IfcRepresentationItem a, IfcRepresentationItem b)
        {
            return a.GeometricEquals(b);
        }

        public int GetHashCode(IfcRepresentationItem obj)
        {
            return obj.GetGeometryHashCode();
        }
    }


    public static class IfcRepresentationItemGeometryExtensions
    {
        
        /// <summary>
        /// Returns true if the object represents a solid model nb FacetedBreps can be both surface and solid
        /// </summary>
        /// <param name="repItem"></param>
        /// <returns></returns>
        public static bool IsSolidModel(this IfcRepresentationItem repItem)
        {

            if (repItem is IfcSolidModel ||
                repItem is IfcBooleanResult ||
                repItem is IfcBoundingBox ||
                repItem is IfcSectionedSpine
                ) return true;
            else return false;
        }

        /// <summary>
        /// Returns true if the object is a surface model, nb FacetedBreps can be both surface and solid
        /// </summary>
        /// <param name="repItem"></param>
        /// <returns></returns>
        public static bool IsSurfaceModel(this IfcRepresentationItem repItem)
        {
            if (repItem is IfcShellBasedSurfaceModel ||
                repItem is IfcFaceBasedSurfaceModel ||
                repItem is IfcFacetedBrep 
                ) return true;
            else return false;
        }

        /// <summary>
        /// Returns true if the item is a map to another shape
        /// </summary>
        /// <param name="repItem"></param>
        /// <returns></returns>
        public static bool IsMappedModel(this IfcRepresentationItem repItem)
        {
            return repItem is IfcMappedItem;
        }


        /// <summary>
        /// Returns a Hash Code for the geometric properties of this object
        /// </summary>
        /// <param name="repItem"></param>
        /// <returns></returns>
        public static int GetGeometryHashCode(this IfcRepresentationItem repItem)
        { 
            if (repItem is IfcSolidModel)
                return ((IfcSolidModel)repItem).GetGeometryHashCode();
            else if (repItem is IfcBooleanResult)
                return ((IfcBooleanResult)repItem).GetGeometryHashCode();
            else if (repItem is IfcConnectedFaceSet)
                return ((IfcConnectedFaceSet)repItem).GetGeometryHashCode();
            else if (repItem is IfcFaceBasedSurfaceModel)
                return ((IfcFaceBasedSurfaceModel)repItem).GetGeometryHashCode();
            else if (repItem is IfcShellBasedSurfaceModel)
                return ((IfcShellBasedSurfaceModel)repItem).GetGeometryHashCode();
            else if (repItem is IfcPlane)
                return ((IfcPlane)repItem).GetGeometryHashCode();
            else if (repItem is IfcBoundingBox)
                return ((IfcBoundingBox)repItem).GetGeometryHashCode();
            else
            {
                throw new XbimGeometryException("Unsupported solid geometry type " + repItem.GetType().Name);
            }
            
        }

        /// <summary>
        /// Compares two objects for geomtric equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">object to compare with</param>
        /// <returns></returns>
        public static bool GeometricEquals(this IfcRepresentationItem a, IfcRepresentationItem b)
        {
            if (a is IfcSolidModel)
                return ((IfcSolidModel)a).GeometricEquals(b);
            else if (a is IfcBooleanResult)
                return ((IfcBooleanResult)a).GeometricEquals(b);
            else if (a is IfcConnectedFaceSet)
                return ((IfcConnectedFaceSet)a).GeometricEquals(b);
            else if (a is IfcFaceBasedSurfaceModel)
                return ((IfcFaceBasedSurfaceModel)a).GeometricEquals(b);
            else if (a is IfcShellBasedSurfaceModel)
                return ((IfcShellBasedSurfaceModel)a).GeometricEquals(b);
            else if (a is IfcPlane)
                return ((IfcPlane)a).GeometricEquals(b);
            else
            {
               // return false;
               throw new XbimGeometryException("Unsupported solid geometry type " + a.GetType().Name);
            }
        }
    }
}
