using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
    /// <summary>
    /// Definition from ISO/CD 10303-46:1992: The light source directional is a subtype of light source. 
    /// This entity has a light source direction. With a conceptual origin at infinity, all the rays of the light 
    /// are parallel to this direction. This kind of light source lights a surface based on the surface's orientation,
    /// but not position.
    /// 
    /// Definition from ISO/IEC 14772-1:1997: The directional light node defines a directional light source that
    /// illuminates along rays parallel to a given 3-dimensional vector. Directional light nodes do not attenuate with distance.
    /// Directional light nodes are specified in the local coordinate system and are affected by ancestor transformations.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcLightSourceDirectional: IfcLightSource
    {
        private IfcDirection _Orientation;

        /// <summary>
        /// Definition from ISO/CD 10303-46:1992: This direction is the direction of the light source.
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: The direction field specifies the direction vector of the 
        /// illumination emanating from the light source in the local coordinate system. Light is emitted along parallel 
        /// rays from an infinite distance away. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcDirection Orientation
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Orientation;
            }
            set { this.SetModelValue(this, ref _Orientation, value, v => Orientation = v, "Orientation"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _Orientation = (IfcDirection)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
