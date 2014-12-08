using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.PresentationResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.PresentationOrganizationResource
{
    /// <summary>
    /// The IfcLightSourceGoniometric defines a light source for which exact lighting data is available. 
    /// It specifies the type of a light emitter, defines the position and orientation of a light distribution curve 
    /// and the data concerning lamp and photometric information.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcLightSourceGoniometric : IfcLightSource
    {
        private IfcAxis2Placement3D _Position;

        /// <summary>
        ///  	The position of the light source. It is used to orientate the light distribution curves.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcAxis2Placement3D Position
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Position;
            }
            set { this.SetModelValue(this, ref _Position, value, v => Position = v, "Position"); }
        }

        private IfcColourRgb _ColourAppearance;

        /// <summary>
        /// Artificial light sources are classified in terms of their color appearance. To the human eye they all appear to be white; the difference can only be detected by direct comparison. Visual performance is not directly affected by differences in color appearance.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcColourRgb ColourAppearance
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ColourAppearance;
            }
            set { this.SetModelValue(this, ref _ColourAppearance, value, v => ColourAppearance = v, "ColourAppearance"); }
        }

        private IfcThermodynamicTemperatureMeasure _ColourTemperature;

        /// <summary>
        /// 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcThermodynamicTemperatureMeasure ColourTemperature
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ColourTemperature;
            }
            set { this.SetModelValue(this, ref _ColourTemperature, value, v => ColourTemperature = v, "ColourTemperature"); }
        }

        private IfcLuminousFluxMeasure _LuminousFlux;

        /// <summary>
        /// Luminous flux is a photometric measure of radiant flux, i.e. the volume of light emitted from a light source. Luminous flux is measured either for the interior as a whole or for a part of the interior (partial luminous flux for a solid angle). All other photometric parameters are derivatives of luminous flux. Luminous flux is measured in lumens (lm). The luminous flux is given as a nominal value for each lamp.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcLuminousFluxMeasure LuminousFlux
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LuminousFlux;
            }
            set { this.SetModelValue(this, ref _LuminousFlux, value, v => LuminousFlux = v, "LuminousFlux"); }
        }

        private IfcLightEmissionSourceEnum _LightEmissionSource;

        /// <summary>
        ///   	Identifies the types of light emitter from which the type required may be set.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Mandatory)]
        public IfcLightEmissionSourceEnum LightEmissionSource
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LightEmissionSource;
            }
            set { this.SetModelValue(this, ref _LightEmissionSource, value, v => LightEmissionSource = v, "LightEmissionSource"); }
        }


        private IfcLightDistributionDataSourceSelect _LightDistributionDataSource;

        /// <summary>
        /// The data source from which light distribution data is obtained.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcLightDistributionDataSourceSelect LightDistributionDataSource
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _LightDistributionDataSource;
            }
            set { this.SetModelValue(this, ref _LightDistributionDataSource, value, v => LightDistributionDataSource = v, "LightDistributionDataSource"); }
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
                    _Position = (IfcAxis2Placement3D)value.EntityVal;
                    break;
                case 5:
                    _ColourAppearance = (IfcColourRgb)value.EntityVal;
                    break;
                case 6:
                    _ColourTemperature = value.RealVal;
                    break;
                case 7:
                    _LuminousFlux = value.RealVal;
                    break;
                case 8:
                    _LightEmissionSource = (IfcLightEmissionSourceEnum)Enum.Parse(typeof(IfcLightEmissionSourceEnum), value.EnumVal);
                    break;
                case 9:
                    _LightDistributionDataSource = (IfcLightDistributionDataSourceSelect)value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
