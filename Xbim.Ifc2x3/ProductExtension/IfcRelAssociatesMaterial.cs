#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssociatesMaterial.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   Objectified relationship between a material definition and elements or element types to which this material definition applies.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: Objectified relationship between a material definition and elements or element types to which this material definition applies. The material definition can be:
    ///   A single material 
    ///   A material list, e.g. for composite elements 
    ///   A material layer set, for layered elements with an indication of the layering direction and individual layer thicknesses 
    ///   The IfcRelAssociatesMaterial relationship is a special type of the IfcRelAssociates relationship. It can be applied to subtypes of IfcElement and subtypes of IfcElementType.
    ///   The IfcElement has an inverse relation to its material definition by the HasAssociations attribute, inherited from IfcObject. 
    ///   The IfcElementType has an inverse relation to its material definition by the HasAssociations attribute, inherited from IfcPropertyDefinition. 
    ///   If there are several different material assignments to a single element (e.g. to give different materials to parts of the element) the inherited Name attribute may provide additional information about the element part to which the material applies. If both, the element occurrence (by an instance of IfcElement) and the element type (by an instance of IfcElementType, connected through IfcRelDefinesByType) have an associated material, then the material associated to the element occurrence overrides the material associated to the element type.
    ///   HISTORY New entity in IFC Release 2.x.
    ///   Formal Propositions:
    ///   WR21   :   The material information must not be associated to a substraction feature (such as an opening) or to a virtual element.  
    ///   WR22   :   The material information, using IfcMaterialSelect should be associated to a product occurrence.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssociatesMaterial : IfcRelAssociates
    {
        #region Fields

        private IfcMaterialSelect _relatingMaterial;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Material definition (either a single material, a list of materials, or a set of material layers) assigned to the elements.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcMaterialSelect RelatingMaterial
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingMaterial;
            }
            set
            {
                this.SetModelValue(this, ref _relatingMaterial, value, v => RelatingMaterial = v,
                                           "RelatingMaterial");
            }
        }


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
                    _relatingMaterial = (IfcMaterialSelect) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            if (RelatedObjects.Where(o => o is IfcFeatureElementSubtraction || o is IfcVirtualElement).Count() > 0)
                baseErr +=
                    "WR21 RelAssociatesMaterial: The material information must not be associated to a substraction feature (such as an opening) or to a virtual element.";
            if (RelatedObjects.Where(o => !(o is IfcProduct || o is IfcTypeProduct)).Count() > 0)
                baseErr +=
                    "WR22 RelAssociatesMaterial: The material information, using MaterialSelect should be associated to a product occurrence.";
            return baseErr;
        }
    }
}