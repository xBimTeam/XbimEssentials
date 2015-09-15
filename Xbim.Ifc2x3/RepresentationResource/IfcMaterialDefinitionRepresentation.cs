#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMaterialDefinitionRepresentation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The IfcMaterialDefinitionRepresentation defines presentation information relating to IfcMaterial.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcMaterialDefinitionRepresentation defines presentation information relating to IfcMaterial. It allows for multiple presentations of the same material for different geometric representation contexts.
    ///   NOTE  The IfcMaterialDefinitionRepresentation is currently only used to define presentation information to material used at subtypes of IfcElement. The IfcMaterial is assigned to the subtype of IfcElement using the IfcRelAssociatesMaterial (eventually via other material related entities IfcMaterialLayerSetUsage, IfcMaterialLayerSet, IfcMaterialLayer). 
    ///   HISTORY  New entity in IFC Release 2x Edition 3.
    ///   IFC2x Edition 3 CHANGE  The entity IfcMaterialDefinitionRepresentation  has been added. Upward compatibility for file based exchange is guaranteed.
    ///   Formal Propositions:
    ///   WR11   :   Only representations of type IfcStyledRepresentation should be used to represent material through the IfcMaterialRepresentation.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcMaterialDefinitionRepresentation : IfcProductRepresentation
    {
        #region Fields

        private IfcMaterial _representedMaterial;

        #endregion

        #region Constructors

        #endregion

        /// <summary>
        ///   Reference to the material to which the representation applies.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory),IndexedProperty]
        public IfcMaterial RepresentedMaterial
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _representedMaterial;
            }
            set
            {
                this.SetModelValue(this, ref _representedMaterial, value, v => RepresentedMaterial = v,
                                           "RepresentedMaterial");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                    base.IfcParse(propIndex, value);
                    break;
                case 3:
                    _representedMaterial = (IfcMaterial) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            if (Representations.OfType<IfcStyledRepresentation>().Count() != Representations.Count)
                return
                    "WR11 MaterialDefinitionRepresentation: Only representations of type StyledRepresentation should be used to represent material through the MaterialRepresentation. ";
            else
                return "";
        }

        #endregion
    }
}