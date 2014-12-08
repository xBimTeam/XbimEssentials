#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStyledItem.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    /// <summary>
    ///   The styled item is an assignment of style for presentation to a geometric representation item as it is used in a representation.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-46:1992: The styled item is an assignment of style for presentation to a geometric representation item as it is used in a representation.
    ///   Definition from IAI: The IfcStyledItem holds presentation style information for products, either explicitly for an IfcGeometricRepresentationItem being part of an IfcShapeRepresentation assigned to a product, or by assigning presentation information to IfcMaterial being assigned as other representation for a product.
    ///   NOTE  Corresponding STEP name: styled_item. Please refer to ISO/IS 10303-46:1994 for the final definition of the formal standard. 
    ///   HISTORY  New entity in Release IFC2x Edition 2. 
    ///   IFC2x Edition 2 Addendum 1 CHANGE  The entity IfcStyledItem has been made non abstract and the attribute Name has been promoted from subtype IfcAnnotationOccurrence. Upward compatibility for file based exchange is guaranteed.
    ///   IFC2x Edition 3 CHANGE  The attribute Item has been made optional, upward compatibility for file based exchange is guaranteed.
    ///   Formal Propositions:
    ///   WR11   :   Restricts the number of styles to 1 (the datatype SET remains for compatibility reasons with ISO 10303-46).  
    ///   WR12   :   A styled item cannot be styled by another styled item.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcStyledItem : IfcRepresentationItem
    {
        public IfcStyledItem()
        {
            _styles = new XbimSet<IfcPresentationStyleAssignment>(this);
        }

        #region Fields

        private IfcRepresentationItem _item;
        private XbimSet<IfcPresentationStyleAssignment> _styles;
        private IfcLabel? _name;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Optional.   A geometric representation item to which the style is assigned.
        /// </summary>
        [IndexedProperty]
        [IfcAttribute(1, IfcAttributeState.Optional)]
        public IfcRepresentationItem Item
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _item;
            }
            set { this.SetModelValue(this, ref _item, value, v => Item = v, "Item"); }
        }


        /// <summary>
        ///   Representation style assignments which are assigned to an item. NOTE: In current IFC release only one presentation style assignment shall be assigned.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1, 1)]
        public XbimSet<IfcPresentationStyleAssignment> Styles
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _styles;
            }
            set { this.SetModelValue(this, ref _styles, value, v => Styles = v, "Styles"); }
        }


        /// <summary>
        ///   Optional.   The word, or group of words, by which the styled item is referred to.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLabel? Name
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _name;
            }
            set { this.SetModelValue(this, ref _name, value, v => Name = v, "Name"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _item = (IfcRepresentationItem) value.EntityVal;
                    break;
                case 1:
                    _styles.Add((IfcPresentationStyleAssignment) value.EntityVal);
                    break;
                case 2:
                    _name = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string err = "";
            if (Styles.Count != 1)
                err +=
                    "WR11 StyledItem : Restricts the number of styles to 1 (the datatype SET remains for compatibility reasons with ISO 10303-46).\n";
            if (_item is IfcStyledItem)
                err += "WR12 StyledItem : A styled item cannot be styled by another styled item. \n";
            return err;
        }

        #endregion
    }
}