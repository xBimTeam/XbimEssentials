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
    /// Definition from ISO/CD 10303-46:1992: The light source positional entity is a subtype of light source. This entity has a light source position and attenuation coefficients. A positional light source affects a surface based on the surface's orientation and position.
    /// 
    /// Definition from ISO/IEC 14772-1:1997: The Point light node specifies a point light source at a 3D location in the local coordinate system. A point light source emits light equally in all directions; that is, it is omnidirectional. Point light nodes are specified in the local coordinate system and are affected by ancestor transformations.
    /// 
    /// Point light node's illumination falls off with distance as specified by three attenuation coefficients. The attenuation factor is
    /// 
    ///     1/max(attenuation[0] + attenuation[1] × r + attenuation[2] × r 2 , 1), 
    /// 
    /// where r is the distance from the light to the surface being illuminated. The default is no attenuation. An attenuation value of (0, 0, 0) is identical to (1, 0, 0). Attenuation values shall be greater than or equal to zero. 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcLightSourcePositional : IfcLightSource
    {
        private IfcCartesianPoint _Position;

        /// <summary>
        /// Definition from ISO/CD 10303-46:1992: The Cartesian point indicates the position of the light source.
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: A Point light node illuminates geometry within radius of its location.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcCartesianPoint Position
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Position;
            }
            set { this.SetModelValue(this, ref _Position, value, v => Position = v, "Position"); }
        }

        private IfcPositiveLengthMeasure _Radius;

        /// <summary>
        /// Definition from IAI: The maximum distance from the light source for a surface still to be illuminated.
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: A Point light node illuminates geometry within radius of its location.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcPositiveLengthMeasure Radius
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Radius;
            }
            set { this.SetModelValue(this, ref _Radius, value, v => Radius = v, "Radius"); }
        }

        private IfcReal _ConstantAttenuation;

        /// <summary>
        /// Definition from ISO/CD 10303-46:1992: This real indicates the value of the attenuation in the lighting equation that is constant.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcReal ConstantAttenuation
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ConstantAttenuation;
            }
            set { this.SetModelValue(this, ref _ConstantAttenuation, value, v => ConstantAttenuation = v, "ConstantAttenuation"); }
        }

        private IfcReal _DistanceAttenuation;

        /// <summary>
        /// Definition from ISO/CD 10303-46:1992: This real indicates the value of the attenuation in the lighting equation that proportional to the distance from the light source.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcReal DistanceAttenuation
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _DistanceAttenuation;
            }
            set { this.SetModelValue(this, ref _DistanceAttenuation, value, v => DistanceAttenuation = v, "DistanceAttenuation"); }
        }

        private IfcReal _QuadricAttenuation;

        /// <summary>
        /// Definition from the IAI: This real indicates the value of the attenuation in the lighting equation that proportional to the square value of the distance from the light source.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcReal QuadricAttenuation
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _QuadricAttenuation;
            }
            set { this.SetModelValue(this, ref _QuadricAttenuation, value, v => QuadricAttenuation = v, "QuadricAttenuation"); }
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
                    _Position = (IfcCartesianPoint)value.EntityVal;
                    break;
                case 5:
                    _Radius = value.RealVal;
                    break;
                case 6:
                    _ConstantAttenuation = value.RealVal;
                    break;
                case 7:
                    _DistanceAttenuation = value.RealVal;
                    break;
                case 8:
                    _QuadricAttenuation = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
