#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSoundProperties.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    [IfcPersistedEntityAttribute]
    public class IfcSoundProperties : IfcPropertySetDefinition
    {
        #region Fields

        private Boolean _isAttenuating;
        private IfcSoundScaleEnum? _soundScale;
        private XbimList<IfcSoundValue> _soundValues;

        #endregion

        /// <summary>
        ///   If TRUE, values represent sound attenuation. If FALSE, values represent sound generation.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public Boolean IsAttenuating
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _isAttenuating;
            }
            set { this.SetModelValue(this, ref _isAttenuating, value, v => IsAttenuating = v, "IsAttenuating"); }
        }

        /// <summary>
        ///   Reference sound scale
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Optional)]
        public IfcSoundScaleEnum? SoundScale
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _soundScale;
            }
            set { this.SetModelValue(this, ref _soundScale, value, v => SoundScale = v, "SoundScale"); }
        }

        /// <summary>
        ///   Sound values at a specific frequency. There may be cases where less than eight values are specified.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory, IfcAttributeType.List, IfcAttributeType.Class, 1, 8)]
        public XbimList<IfcSoundValue> SoundValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _soundValues;
            }
            set { this.SetModelValue(this, ref _soundValues, value, v => SoundValues = v, "SoundValues"); }
        }

        #region ISupportIfcParser Members

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
                    _isAttenuating = value.BooleanVal;
                    break;
                case 5:
                    _soundScale = (IfcSoundScaleEnum) Enum.Parse(typeof (IfcSoundScaleEnum), value.EnumVal, true);
                    break;
                case 6:
                    _soundValues.Add((IfcSoundValue) value.EntityVal);
                    break;

                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public override string WhereRule()
        {
            return "";
        }

        #endregion
    }
}