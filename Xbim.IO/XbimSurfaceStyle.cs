using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.PresentationAppearanceResource;
using Xbim.XbimExtensions;

namespace Xbim.IO
{
    /// <summary>
    /// Represents a material used to render a surface of a geometry
    /// </summary>
    public struct XbimSurfaceStyle
    {
        private int styleId;
        private short ifcTypeId;

        /// <summary>
        /// Holds the material used by the graphics engine to render the surface style
        /// Set to a value to suite  specific needs of the graphics environment being used
        /// </summary>
        public object TagRenderMaterial;
        ///// <summary>
        ///// List of Geometry data objects rendererd by this style
        ///// </summary>
        //public List<XbimGeometryData> GeometryData = new List<XbimGeometryData>();


        public short IfcTypeId
        {
            get { return ifcTypeId; }
        }

        public XbimSurfaceStyle(short ifcTypeId, int ifcSurfaceStyleId)
        {

            this.ifcTypeId = ifcTypeId;
            this.styleId = ifcSurfaceStyleId;
            this.TagRenderMaterial = null;
        }


        public int IfcSurfaceStyleLabel
        {
            get { return styleId; }
          
        }

        public IfcSurfaceStyle IfcSurfaceStyle(XbimModel model)
        {
            if (IsIfcSurfaceStyle) return (IfcSurfaceStyle)model.Instances[styleId]; else return null;
        }

        public IfcType IfcType
        {
            get { return IfcMetaData.IfcType(ifcTypeId); }
           
        }

        public bool IsIfcSurfaceStyle
        {
            get
            {
                return styleId > 0;
            }
        }


        public override int GetHashCode()
        {
            if (IsIfcSurfaceStyle) return styleId; else return ifcTypeId * -1;
        }

        public override bool Equals(object obj)
        {
            
            if (obj is XbimSurfaceStyle)
            {
                XbimSurfaceStyle compareTo = (XbimSurfaceStyle) obj;
                if (IsIfcSurfaceStyle && styleId == compareTo.styleId) //if it is a surface style then this takes priority
                    return true;
                else if (IsIfcSurfaceStyle)
                    return false;
                else
                    return ifcTypeId == compareTo.ifcTypeId; //otherwise the ifc type is precedent
            }
            else
                return false;
           
        }


    }
}
