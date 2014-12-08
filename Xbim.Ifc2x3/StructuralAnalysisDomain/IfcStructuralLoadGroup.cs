#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralLoadGroup.cs
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
    ///   The entity IfcStructuralLoadGroup is used to structure the physical impacts. 
    ///   By using the grouping features inherited from IfcGroup, instances of IfcStructuralAction (or its subclasses) 
    ///   and of IfcStructuralLoadGroup can be used to define load groups, load cases and load combinations. 
    ///   An optional coefficient can be provided to represent safety factors known from several codes of practice. 
    ///   (see also IfcLoadGroupTypeEnum)
    ///   NOTE: Important functionality for the description of a load-bearing system is derived from the existing IFC entity IfcGroup. 
    ///   This class provides, via the relationship class IfcRelAssignsToGroup, the needed grouping mechanism. 
    ///   In this way, instances of IfcStructuralAction belonging to a specific load group can be unambiguously determined.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcStructuralLoadGroup : IfcGroup
    {
        #region Fields

        private IfcLoadGroupTypeEnum _predefinedType;
        private IfcActionTypeEnum _actionType;
        private IfcActionSourceTypeEnum _actionSource;
        private IfcRatioMeasure? _coefficient;
        private IfcLabel? _purpose;

        #endregion

        #region Properties

        /// <summary>
        ///   Selects a predefined type for the load group. It can be differentiated between load groups, load cases, load combination groups 
        ///   (a necessary construct for the description of load combinations) and load combinations.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcLoadGroupTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
        }

        /// <summary>
        ///   Type of actions in the group. Normally needed if 'PredefinedType' specifies a LOAD_COMBINATION_GROUP.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcActionTypeEnum ActionType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _actionType;
            }
            set { this.SetModelValue(this, ref _actionType, value, v => ActionType = v, "ActionType"); }
        }

        /// <summary>
        ///   Source of actions in the group. Normally needed if 'PredefinedType' specifies a LOAD_CASE.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcActionSourceTypeEnum ActionSource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _actionSource;
            }
            set { this.SetModelValue(this, ref _actionSource, value, v => ActionSource = v, "ActionSource"); }
        }

        /// <summary>
        ///   Load factor. If omitted, a factor is not yet known or not specified. A load factor of 1.0 shall be explicitly exported as Coefficient = 1.0.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcRatioMeasure? Coefficient
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _coefficient;
            }
            set { this.SetModelValue(this, ref _coefficient, value, v => Coefficient = v, "Coefficient"); }
        }

        /// <summary>
        ///   Description of the purpose of this instance. Among else, possible values of the Purpose of load combinations are 'SLS', 'ULS', 'ALS' to 
        ///   indicate serviceability, ultimate, or accidental limit state.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcLabel? Purpose
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _purpose;
            }
            set { this.SetModelValue(this, ref _purpose, value, v => Purpose = v, "Purpose"); }
        }

        /// <summary>
        ///   Results which were computed using this load group.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set, 0, 1)]
        public IEnumerable<IfcStructuralResultGroup> SourceOfResultGroup
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcStructuralResultGroup>(
                        s => s.ResultForLoadGroup == this);
            }
        }

        /// <summary>
        ///   Analysis models in which this load group is used.
        /// </summary>
        [IfcAttribute(-1, IfcAttributeState.Mandatory, IfcAttributeType.Set)]
        public IEnumerable<IfcStructuralAnalysisModel> LoadGroupFor
        {
            get
            {
                return
                    ModelOf.Instances.Where<IfcStructuralAnalysisModel>(s => s.LoadedBy.Contains(this));
            }
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
                    _predefinedType =
                        (IfcLoadGroupTypeEnum) Enum.Parse(typeof (IfcLoadGroupTypeEnum), value.EnumVal, true);
                    break;
                case 6:
                    _actionType = (IfcActionTypeEnum) Enum.Parse(typeof (IfcActionTypeEnum), value.EnumVal, true);
                    break;
                case 7:
                    _actionSource =
                        (IfcActionSourceTypeEnum) Enum.Parse(typeof (IfcActionSourceTypeEnum), value.EnumVal, true);
                    break;
                case 8:
                    _coefficient = value.RealVal;
                    break;
                case 9:
                    _purpose = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}