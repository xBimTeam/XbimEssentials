using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
    /// <summary>
    /// Definition from ISO/CD 10303-46:1992: The light source spot entity is a subtype of light source. Spot light source entities have a light source colour, position, direction, attenuation coefficients, concentration exponent, and spread angle. If a point lies outside the cone of influence of a light source of this type as determined by the light source position, direction and spread angle its colour is not affected by that light source.
    /// 
    ///     NOTE: The IfcLightSourceSpot adds the BeamWidthAngle which defines the inner cone in which the light source emits light at uniform full intensity. The light source's emission intensity drops off from the inner solid angle (BeamWidthAngle) to the outer solid angle (SpreadAngle).
    /// 
    /// Definition from ISO/IEC 14772-1:1997: The Spot light node defines a light source that emits light from a specific point along a specific direction vector and constrained within a solid angle. Spot lights may illuminate geometry nodes that respond to light sources and intersect the solid angle defined by the Spot light. Spot light nodes are specified in the local coordinate system and are affected by ancestors' transformations.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcLightSourceSpot : IfcLightSourcePositional
    {
        private IfcDirection _Orientation;

        /// <summary>
        /// Definition from ISO/CD 10303-46:1992: This is the direction of the axis of the cone of the light source specified in the coordinate space of the representation being projected..
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: The direction field specifies the direction vector of the light's central axis defined in the local coordinate system.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcDirection Orientation
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Orientation;
            }
            set { this.SetModelValue(this, ref _Orientation, value, v => Orientation = v, "Orientation"); }
        }

        private IfcReal? _ConcentrationExponent;

        /// <summary>
        /// Definition from ISO/CD 10303-46:1992: This real is the exponent on the cosine of the angle between the line that starts at the position of the spot light source and is in the direction of the orientation of the spot light source and a line that starts at the position of the spot light source and goes through a point on the surface being shaded.
        /// NOTE: This attribute does not exists in ISO/IEC 14772-1:1997. 
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcReal? ConcentrationExponent
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ConcentrationExponent;
            }
            set { this.SetModelValue(this, ref _ConcentrationExponent, value, v => ConcentrationExponent = v, "ConcentrationExponent"); }
        }

        private IfcPositivePlaneAngleMeasure _SpreadAngle;

        /// <summary>
        /// Definition from ISO/CD 10303-46:1992: This planar angle measure is the angle between the line that starts at the position of the spot light source and is in the direction of the spot light source and any line on the boundary of the cone of influence.
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: The cutOffAngle (name of spread angle in VRML) field specifies the outer bound of the solid angle. The light source does not emit light outside of this solid angle.
        /// </summary>
        [IfcAttribute(12, IfcAttributeState.Mandatory)]
        public IfcPositivePlaneAngleMeasure SpreadAngle
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _SpreadAngle;
            }
            set { this.SetModelValue(this, ref _SpreadAngle, value, v => SpreadAngle = v, "SpreadAngle"); }
        }

        private IfcPositivePlaneAngleMeasure _BeamWidthAngle;

        /// <summary>
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: The beamWidth field specifies an inner solid angle in which the light source emits light at uniform full intensity. The light source's emission intensity drops off from the inner solid angle (beamWidthAngle) to the outer solid angle (spreadAngle).
        /// </summary>
        [IfcAttribute(13, IfcAttributeState.Mandatory)]
        public IfcPositivePlaneAngleMeasure BeamWidthAngle
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _BeamWidthAngle;
            }
            set { this.SetModelValue(this, ref _BeamWidthAngle, value, v => BeamWidthAngle = v, "BeamWidthAngle"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _Orientation = (IfcDirection)value.EntityVal;
                    break;
                case 10:
                    _ConcentrationExponent = value.RealVal;
                    break;
                case 11:
                    _SpreadAngle = value.RealVal;
                    break;
                case 12:
                    _BeamWidthAngle = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
