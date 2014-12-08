#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralResultGroup.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   Instances of the entity IfcStructuralResultGroup are used to group results of structural analysis calculations 
    ///   and to capture the connection to the underlying basic load group. 
    ///   The basic functionality for grouping inherited from IfcGroup is used to collect instances from IfcStructuralReaction or 
    ///   its respective subclasses.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcStructuralResultGroup : IfcGroup
    {
        #region Fields

        private IfcAnalysisTheoryTypeEnum _theoryType;
        private IfcStructuralLoadGroup _resultForLoadGroup;
        private IfcBoolean _isLinear;

        #endregion

        #region Properties

        /// <summary>
        ///   Specifies the analysis theory used to obtain the respective results.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcAnalysisTheoryTypeEnum TheoryType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _theoryType;
            }
            set { this.SetModelValue(this, ref _theoryType, value, v => TheoryType = v, "TheoryType"); }
        }

        /// <summary>
        ///   Reference to an instance of IfcStructuralLoadGroup for which this instance represents the result.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcStructuralLoadGroup ResultForLoadGroup
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _resultForLoadGroup;
            }
            set
            {
                this.SetModelValue(this, ref _resultForLoadGroup, value, v => ResultForLoadGroup = v,
                                           "ResultForLoadGroup");
            }
        }

        /// <summary>
        ///   This Boolean value allows to easily recognize if a linear analysis has been applied (allowing the superposition of analysis results), or vice versa.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcBoolean IsLinear
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _isLinear;
            }
            set { this.SetModelValue(this, ref _isLinear, value, v => IsLinear = v, "IsLinear"); }
        }

        /// <summary>
        ///   Reference to an instance of IfcStructuralAnalysisModel for which this instance captures a result
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, 0, 1)]
        public IEnumerable<IfcStructuralAnalysisModel> ResultGroupFor
        {
            get { return ModelOf.Instances.Where<IfcStructuralAnalysisModel>(s => s.HasResults == this); }
        }

        #endregion

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    base.IfcParse(propIndex, value);
                    break;
                case 5:
                    _theoryType =
                        (IfcAnalysisTheoryTypeEnum)
                        Enum.Parse(typeof (IfcAnalysisTheoryTypeEnum), value.EnumVal, true);
                    break;
                case 6:
                    _resultForLoadGroup = (IfcStructuralLoadGroup) value.EntityVal;
                    break;
                case 7:
                    _isLinear = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}