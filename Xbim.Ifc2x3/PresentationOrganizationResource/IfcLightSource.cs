using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
    /// <summary>
    /// The light source entity is determined by the reflectance specified in the surface style rendering. 
    /// Lighting is applied on a surface by surface basis: no interactions between surfaces such as shadows or reflections are defined.
    /// </summary>
    [IfcPersistedEntity]
    public abstract class IfcLightSource : IfcGeometricRepresentationItem
    {
        private IfcLabel? _Name;

        /// <summary>
        /// The name given to the light source in presentation.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcLabel? Name
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Name;
            }
            set { this.SetModelValue(this, ref _Name, value, v => Name = v, "Name"); }
        }

        private IfcColourRgb _LightColour;

        /// <summary>
        /// Definition from ISO/CD 10303-46:1992: Based on the current lighting model, the colour of the light to be used for shading.
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: The color field specifies the spectral color properties of both the direct and ambient light 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcColourRgb LightColour
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LightColour;
            }
            set { this.SetModelValue(this, ref _LightColour, value, v => LightColour = v, "LightColour"); }
        }

        private IfcNormalisedRatioMeasure? _AmbientIntensity;

        /// <summary>
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: The ambientIntensity specifies the intensity of the ambient emission from the light. 
        /// Light intensity may range from 0.0 (no light emission) to 1.0 (full intensity). 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? AmbientIntensity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _AmbientIntensity;
            }
            set { this.SetModelValue(this, ref _AmbientIntensity, value, v => AmbientIntensity = v, "AmbientIntensity"); }
        }

        private IfcNormalisedRatioMeasure? _Intensity;

        /// <summary>
        /// Definition from VRML97 - ISO/IEC 14772-1:1997: The intensity field specifies the brightness of the direct emission from the ligth. 
        /// Light intensity may range from 0.0 (no light emission) to 1.0 (full intensity).
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? Intensity
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Intensity;
            }
            set { this.SetModelValue(this, ref _Intensity, value, v => Intensity = v, "Intensity"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _Name = value.StringVal;
                    break;
                case 1:
                    _LightColour = (IfcColourRgb)value.EntityVal;
                    break;
                case 2:
                    _AmbientIntensity = value.RealVal;
                    break;
                case 3:
                    _Intensity = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}
