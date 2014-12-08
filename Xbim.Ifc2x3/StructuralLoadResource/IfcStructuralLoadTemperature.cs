#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadTemperature.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralLoadResource
{
    [IfcPersistedEntityAttribute]
    public class IfcStructuralLoadTemperature : IfcStructuralLoadStatic
    {
        #region Fields

        private IfcThermodynamicTemperatureMeasure? _deltaT_Constant;
        private IfcThermodynamicTemperatureMeasure? _deltaT_Y;
        private IfcThermodynamicTemperatureMeasure? _deltaT_Z;

        #endregion

        #region Properties

        /// <summary>
        ///   Temperature change which is applied to the complete section of the structural member. A positive value describes an increase in temperature.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Optional)]
        public IfcThermodynamicTemperatureMeasure? DeltaT_Constant
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _deltaT_Constant;
            }
            set
            {
                this.SetModelValue(this, ref _deltaT_Constant, value, v => DeltaT_Constant = v,
                                           "DeltaT_Constant");
            }
        }

        /// <summary>
        ///   Temperature change which is applied to the outer fiber of the positive Y-direction. A positive value describes an increase in temperature.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcThermodynamicTemperatureMeasure? DeltaT_Y
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _deltaT_Y;
            }
            set { this.SetModelValue(this, ref _deltaT_Y, value, v => DeltaT_Y = v, "DeltaT_Y"); }
        }

        /// <summary>
        ///   Temperature change which is applied to the outer fiber of the positive Z-direction. A positive value describes an increase in temperature.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Optional)]
        public IfcThermodynamicTemperatureMeasure? DeltaT_Z
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _deltaT_Z;
            }
            set { this.SetModelValue(this, ref _deltaT_Z, value, v => DeltaT_Z = v, "DeltaT_Z"); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _deltaT_Constant = value.RealVal;
                    break;
                case 2:
                    _deltaT_Y = value.RealVal;
                    break;
                case 3:
                    _deltaT_Z = value.RealVal;
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