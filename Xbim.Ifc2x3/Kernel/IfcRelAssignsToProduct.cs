#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssignsToProduct.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   This objectified relationship IfcRelAssignsToProduct handles the assignment of objects (subtypes of IfcObject) to a product (subtypes of IfcProduct).
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This objectified relationship IfcRelAssignsToProduct handles the assignment of objects (subtypes of IfcObject) to a product (subtypes of IfcProduct). 
    ///   The Name attribute should be used to classify the usage of the IfcRelAssignsToProduct objectified relationship. The following Name values are proposed:
    ///   'Reference' : Assignment of a product (via RelatingProduct) to a spatial structure (via RelatedObjects) to which it is referenced (in contrary to being contained - which is handled by IfcRelContainedInSpatialStructure). 
    ///   IFC2x Edition 3 CHANGE  The reference of a product within a spatial structure is now handled by a new relationship object IfcRelReferencedInSpatialStructure. The IfcRelAssignsToProduct shall not be used to represent this relation from IFC2x3 onwards.
    ///   'Context' : Assignment of a context specific representation, such as of structural members to a different context representation (with potentially different decomposition breakdown) such as of building elements for a specific context specific representation.  
    ///   IFC2x Edition 3 CHANGE  The relation of a structural member (as instance of IfcStructuralMember or its subclasses) to a physical element  (as instance of IfcElement or its subclasses) is now handled by a new relationship object IfcRelConnectsStructuralElement. The IfcRelAssignsToProduct shall not be used to represent this relation from IFC2x3 onwards.
    ///   'View' : Assignment of a product (via RelatingProduct) that is decomposed according to a discipline view, to another product (via RelatedObjects) that is decomposed according to a different discipline view. An example is the assignment of the architectural slab to a different decomposition of the pre manufactured sections of a slab (under a precast concrete discipline view). 
    ///   HISTORY New Entity in IFC Release 2x
    ///   Formal Propositions:
    ///   WR1   :   The instance to which the relation points (RelatingProduct) shall not be contained in the list of RelatedObjects.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelAssignsToProduct : IfcRelAssigns
    {
        #region Fields

        private IfcProduct _relatingProduct;

        #endregion

        /// <summary>
        ///   Reference to group that finally contains all assigned group members.
        /// </summary>
        /// <remarks>
        ///   WR1   :   The instance to with the relation points shall not be contained in the List of RelatedObjects.
        /// </remarks>
        [IfcAttribute(7, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcProduct RelatingProduct
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingProduct;
            }
            set
            {
                this.SetModelValue(this, ref _relatingProduct, value, v => RelatingProduct = v,
                                           "RelatingProduct");
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
                case 5:
                    base.IfcParse(propIndex, value);
                    break;
                case 6:
                    _relatingProduct = (IfcProduct) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}