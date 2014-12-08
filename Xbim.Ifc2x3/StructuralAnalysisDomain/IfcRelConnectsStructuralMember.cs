#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsStructuralMember.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.StructuralLoadResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsStructuralMember : IfcRelConnects
    {
        #region Fields

        private IfcStructuralMember _relatingStructuralMember;
        private IfcStructuralConnection _relatedStructuralConnection;
        private IfcBoundaryCondition _appliedCondition;
        private IfcStructuralConnectionCondition _additionalConditions;
        private IfcLengthMeasure? _supportedLength;
        private IfcAxis2Placement3D _conditionCoordinateSystem;

        #endregion

        #region Properties

        /// <summary>
        ///   Reference to an instance of IfcStructuralMember (or its subclasses) which is connected to the specified structural connection.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcStructuralMember RelatingStructuralMember
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingStructuralMember;
            }
            set
            {
                this.SetModelValue(this, ref _relatingStructuralMember, value, v => RelatingStructuralMember = v,
                                           "RelatingStructuralMember");
            }
        }

        /// <summary>
        ///   Reference to an instance of IfcStructuralConnection (or its subclasses) which is connected to the specified structural member.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcStructuralConnection RelatedStructuralConnection
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedStructuralConnection;
            }
            set
            {
                this.SetModelValue(this, ref _relatedStructuralConnection, value,
                                           v => RelatedStructuralConnection = v, "RelatedStructuralConnection");
            }
        }

        /// <summary>
        ///   Reference to an instance of IfcBoundaryCondition which is used to define the connections properties.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcBoundaryCondition AppliedCondition
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _appliedCondition;
            }
            set
            {
                this.SetModelValue(this, ref _appliedCondition, value, v => AppliedCondition = v,
                                           "AppliedCondition");
            }
        }

        /// <summary>
        ///   Reference to instances describing additional connection properties.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Optional)]
        public IfcStructuralConnectionCondition AdditionalConditions
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _additionalConditions;
            }
            set
            {
                this.SetModelValue(this, ref _additionalConditions, value, v => AdditionalConditions = v,
                                           "AdditionalConditions");
            }
        }

        /// <summary>
        ///   Defines the 'supported length' of this structural connection.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcLengthMeasure? SupportedLength
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _supportedLength;
            }
            set
            {
                this.SetModelValue(this, ref _supportedLength, value, v => SupportedLength = v,
                                           "SupportedLength");
            }
        }

        /// <summary>
        ///   Defines a new coordinate system used for the description of the connection properties.
        ///   The usage of this coordinate system is described more detailed in the definition of the subtypes of this entity definition.
        /// </summary>
        [IfcAttribute(10, IfcAttributeState.Optional)]
        public IfcAxis2Placement3D ConditionCoordinateSystem
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _conditionCoordinateSystem;
            }
            set
            {
                this.SetModelValue(this, ref _conditionCoordinateSystem, value,
                                           v => ConditionCoordinateSystem = v, "ConditionCoordinateSystem");
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
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _relatingStructuralMember = (IfcStructuralMember) value.EntityVal;
                    break;
                case 5:
                    _relatedStructuralConnection = (IfcStructuralConnection) value.EntityVal;
                    break;
                case 6:
                    _appliedCondition = (IfcBoundaryCondition) value.EntityVal;
                    break;
                case 7:
                    _additionalConditions = (IfcStructuralConnectionCondition) value.EntityVal;
                    break;
                case 8:
                    _supportedLength = value.RealVal;
                    break;
                case 9:
                    _conditionCoordinateSystem = (IfcAxis2Placement3D) value.EntityVal;
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