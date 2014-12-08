#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcObjective.cs
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

namespace Xbim.Ifc2x3.ConstraintResource
{
    public class IfcObjective : IfcConstraint
    {
        #region Fields

        private IfcMetric _benchmarkValues;
        private IfcMetric _resultValues;
        private IfcObjectiveEnum _objectiveQualifier;
        private IfcLabel _userDefinedQualifier;

        #endregion

        /// <summary>
        ///   A list of any benchmark values used for comparison purposes.
        /// </summary>
        [Ifc(8, IfcAttributeState.Optional)]
        public IfcMetric BenchmarkValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _benchmarkValues;
            }
            set
            {
                this.SetModelValue(this, ref _benchmarkValues, value, v => BenchmarkValues = v,
                                           "BenchmarkValues");
            }
        }

        /// <summary>
        ///   A list of any resultant values used for comparison purposes.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcMetric ResultValues
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _resultValues;
            }
            set { this.SetModelValue(this, ref _resultValues, value, v => ResultValues = v, "ResultValues"); }
        }

        /// <summary>
        ///   Enumeration that qualifies the type of objective constraint.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Mandatory)]
        public IfcObjectiveEnum ObjectiveQualifier
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _objectiveQualifier;
            }
            set
            {
                this.SetModelValue(this, ref _objectiveQualifier, value, v => ObjectiveQualifier = v,
                                           "ObjectiveQualifier");
            }
        }

        /// <summary>
        ///   Enumeration that qualifies the type of objective constraint.
        /// </summary>
        [IfcAttribute(11, IfcAttributeState.Optional)]
        public IfcLabel UserDefinedQualifier
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _userDefinedQualifier;
            }
            set
            {
                this.SetModelValue(this, ref _userDefinedQualifier, value, v => UserDefinedQualifier = v,
                                           "UserDefinedQualifier");
            }
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
                case 4:
                case 5:
                case 6:
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _benchmarkValues = (IfcMetric) value.EntityVal;
                    break;
                case 8:
                    _resultValues = (IfcMetric) value.EntityVal;
                    break;
                case 9:
                    _objectiveQualifier = (IfcObjectiveEnum) Enum.Parse(typeof (IfcObjectiveEnum), value.EnumVal);
                    break;
                case 10:
                    _userDefinedQualifier = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (_objectiveQualifier == IfcObjectiveEnum.USERDEFINED && string.IsNullOrEmpty(UserDefinedQualifier))
                baseErr +=
                    "WR21 Objective : The attribute UserDefinedQualifier must be asserted when the value of the IfcObjectiveEnum is set to USERDEFINED.\n";
            return baseErr;
        }
    }
}