#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcColumnType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   The element type (IfcColumnType) defines a list of commonly shared property set definitions of a column and an optional set of product representations.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The element type (IfcColumnType) defines a list of commonly shared property set definitions of a column and an optional set of product representations. It is used to define a column specification (i.e. the specific product information that is common to all occurrences of that product type). 
    ///   NOTE: The product representations are defined as representation maps (at the level of the supertype IfcTypeProduct, which gets assigned by an element occurrence instance through the IfcShapeRepresentation.Item[1] being an IfcMappedItem.
    ///   A column type is used to define the common properties of a certain type of column that may be applied to many instances of that type to assign a specific style. Column types may be exchanged without being already assigned to occurrences.
    ///   The occurrences of the IfcColumnType are represented by instances of IfcColumn.
    ///   HISTORY New entity in Release IFC2x Edition 2.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcColumnType : IfcBuildingElementType
    {
        #region Fields

        private IfcColumnTypeEnum _predefinedType;

        #endregion

        #region Part 21 Step file Parse routines

        [IfcAttribute(10, IfcAttributeState.Mandatory, IfcAttributeType.Enum)]
        public IfcColumnTypeEnum PredefinedType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _predefinedType;
            }
            set { this.SetModelValue(this, ref _predefinedType, value, v => PredefinedType = v, "PredefinedType"); }
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
                case 8:
                    base.IfcParse(propIndex, value);
                    break;
                case 9:
                    _predefinedType = (IfcColumnTypeEnum) Enum.Parse(typeof (IfcColumnTypeEnum), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion
    }
}