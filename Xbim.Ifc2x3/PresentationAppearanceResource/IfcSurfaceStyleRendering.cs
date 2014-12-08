#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurfaceStyleRendering.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    [IfcPersistedEntityAttribute]
    public class IfcSurfaceStyleRendering : IfcSurfaceStyleShading
    {
        #region Fields

        private IfcNormalisedRatioMeasure? _transparency;
        private IfcColourOrFactor _diffuseColour;

        private IfcColourOrFactor _transmissionColour;
        private IfcColourOrFactor _diffuseTransmissionColour;
        private IfcColourOrFactor _reflectionColour;
        private IfcColourOrFactor _specularColour;
        private IfcSpecularHighlightSelect _specularHighlight;
        private IfcReflectanceMethodEnum _reflectanceMethod = IfcReflectanceMethodEnum.NOTDEFINED;

        #endregion

        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcNormalisedRatioMeasure? Transparency
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _transparency;
            }
            set { this.SetModelValue(this, ref _transparency, value, v => Transparency = v, "Transparency"); }
        }

        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcColourOrFactor DiffuseColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _diffuseColour;
            }
            set { this.SetModelValue(this, ref _diffuseColour, value, v => DiffuseColour = v, "DiffuseColour"); }
        }

        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcColourOrFactor TransmissionColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _diffuseTransmissionColour;
            }
            set
            {
                this.SetModelValue(this, ref _transmissionColour, value, v => TransmissionColour = v,
                                           "TransmissionColour");
            }
        }


        [IfcAttribute(5, IfcAttributeState.Optional)]
        public IfcColourOrFactor DiffuseTransmissionColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _diffuseTransmissionColour;
            }
            set
            {
                this.SetModelValue(this, ref _diffuseTransmissionColour, value,
                                           v => DiffuseTransmissionColour = v, "DiffuseTransmissionColour");
            }
        }

        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcColourOrFactor ReflectionColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _reflectionColour;
            }
            set
            {
                this.SetModelValue(this, ref _reflectionColour, value, v => ReflectionColour = v,
                                           "ReflectionColour");
            }
        }

        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcColourOrFactor SpecularColour
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _specularColour;
            }
            set { this.SetModelValue(this, ref _specularColour, value, v => SpecularColour = v, "SpecularColour"); }
        }

        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcSpecularHighlightSelect SpecularHighlight
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _specularHighlight;
            }
            set
            {
                this.SetModelValue(this, ref _specularHighlight, value, v => SpecularHighlight = v,
                                           "SpecularHighlight");
            }
        }

        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcReflectanceMethodEnum ReflectanceMethod
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _reflectanceMethod;
            }
            set
            {
                this.SetModelValue(this, ref _reflectanceMethod, value, v => ReflectanceMethod = v,
                                           "ReflectanceMethod");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _transparency = value.RealVal;
                    break;
                case 2:
                    _diffuseColour = (IfcColourOrFactor) value.EntityVal;
                    break;
                case 3:
                    _transmissionColour = (IfcColourOrFactor) value.EntityVal;
                    break;
                case 4:
                    _diffuseTransmissionColour = (IfcColourOrFactor) value.EntityVal;
                    break;
                case 5:
                    _reflectionColour = (IfcColourOrFactor) value.EntityVal;
                    break;
                case 6:
                    _specularColour = (IfcColourOrFactor) value.EntityVal;
                    break;
                case 7:
                    _specularHighlight = (IfcSpecularHighlightSelect) value.EntityVal;
                    break;
                case 8:
                    _reflectanceMethod =
                        (IfcReflectanceMethodEnum) Enum.Parse(typeof (IfcReflectanceMethodEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}