#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElementType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   The element type (IfcElementType) defines a list of commonly shared property set definitions of an element and an optional set of product representations.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The element type (IfcElementType) defines a list of commonly shared property set definitions of an element and an optional 
    ///   set of product representations. It is used to define an element specification (i.e. the specific product information, 
    ///   that is common to all occurrences of that product type).
    ///   NOTE: The product representations are defined as representation maps (at the level of the supertype IfcTypeProduct, 
    ///   which gets assigned by an element instance through the IfcShapeRepresentation.Item[1] being an IfcMappedItem.
    ///   An element type is used to define the common properties of a certain type or style of an element that may be applied
    ///   to instances of that element type to assign a specific style. 
    ///   Element types (the instantiable subtypes) may be exchanged without being already assigned to occurrences.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcElementType : IfcTypeProduct
    {
        #region Fields

        private IfcLabel? _elementType;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The type denotes a particular type that indicates the object further. 
        ///   The use has to be established at the level of instantiable subtypes. In particular it holds the user defined type, 
        ///   if the enumeration of the attribute 'PredefinedType' is set to USERDEFINED.
        /// </summary>
        [IfcAttribute(9, IfcAttributeState.Optional)]
        public IfcLabel? ElementType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _elementType;
            }
            set { this.SetModelValue(this, ref _elementType, value, v => ElementType = v, "ElementType"); }
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
                case 6:
                case 7:
                    base.IfcParse(propIndex, value);
                    break;
                case 8:
                    _elementType = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}