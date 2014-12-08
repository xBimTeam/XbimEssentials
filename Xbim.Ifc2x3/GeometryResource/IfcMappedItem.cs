#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMappedItem.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   Definition from ISO/CD 10303-43:1992: A mapped item is the use of an existing representation (the mapping source - 
    ///   mapped representation) as a representation item in a second representation. 
    ///   NOTE: A mapped item is a subtype of representation item. 
    ///   It enables a representation to be used as a representation item in one or more other representations. 
    ///   The mapped item allows for the definition of a representation using other representations. 
    ///   Definition from IAI: The IfcMappedItem is the inserted instance of a source definition (to be compared with a block / shared cell / macro definition). 
    ///   The instance is inserted by applying a Cartesian transformation operator as the MappingTarget.
    ///   EXAMPLE  An IfcMappedItem can reuse other mapped items (ako nested blocks), 
    ///   doing so the IfcRepresentationMap is based on an IfcShapeRepresentation including one or more IfcMappedItem's.
    ///   NOTE   Corresponding STEP entity: mapped_item. Please refer to ISO/IS 10303-43:1994, for the final definition of the formal standard. 
    ///   The definition of mapping_target (MappingTarget) has been restricted to be of the type cartesian_transformation_operator (IfcCartesianTransformationOperator).
    ///   HISTORY  New entity in IFC Release 2x. 
    ///   Informal Propositions
    ///   A mapped item shall not be self-defining by participating in the definition of the representation being mapped. 
    ///   The dimensionality of the mapping source and the mapping target has to be the same, if the mapping source is a geometric representation item.
    /// </summary>
    [IfcPersistedEntityAttribute,IndexedClass]
    public class IfcMappedItem : IfcRepresentationItem
    {
        #region Fields

        private IfcRepresentationMap _mappingSource;
        private IfcCartesianTransformationOperator _mappingTarget;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   A representation map that is the source of the mapped item. It can be seen as a block (or cell or macro) definition.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcRepresentationMap MappingSource
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _mappingSource;
            }
            set { this.SetModelValue(this, ref _mappingSource, value, v => MappingSource = v, "MappingSource"); }
        }

        /// <summary>
        ///   A representation item that is the target onto which the mapping source is mapped. 
        ///   It is constraint to be a Cartesian transformation operator.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcCartesianTransformationOperator MappingTarget
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _mappingTarget;
            }
            set { this.SetModelValue(this, ref _mappingTarget, value, v => MappingTarget = v, "MappingTarget"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _mappingSource = (IfcRepresentationMap) value.EntityVal;
                    break;
                case 1:
                    _mappingTarget = (IfcCartesianTransformationOperator) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            return "";
        }
    }
}