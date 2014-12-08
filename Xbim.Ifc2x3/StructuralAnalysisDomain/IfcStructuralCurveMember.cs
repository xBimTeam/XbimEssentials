#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralCurveMember.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   Instances of the entity IfcStructuralCurveMember shall be used to describe linear structural elements. Profile and material properties are defined by using objectified relationships:
    ///   The material properties are defined by IfcMechanicalMaterialProperties (and subtypes), they are connected through
    ///   IfcMaterial and IfcRelAssociatesMaterial and are accessible via the inherited inverse relationship HasAssociations. 
    ///   The profile properties are defined by IfcMechanicalProfileProperties (and subtypes), they are connected through 
    ///   IfcRelAssociatesProfileProperties and are accessible via the inherited inverse relationship HasAssociations.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcStructuralCurveMember : IfcStructuralMember
    {
        #region Fields

        private IfcStructuralCurveTypeEnum _predefinedType;

        #endregion

        #region Properties

        /// <summary>
        ///   Defines the load carrying behavior of the member, as far as it is taken into account in the analysis.
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory)]
        public IfcStructuralCurveTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
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
                case 5:
                case 6:
                    base.IfcParse(propIndex, value);
                    break;
                case 7:
                    _predefinedType =
                        (IfcStructuralCurveTypeEnum)
                        Enum.Parse(typeof (IfcStructuralCurveTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}