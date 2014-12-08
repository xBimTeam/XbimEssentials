#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPropertyReferenceValue.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.PropertyResource
{
    /// <summary>
    ///   The IfcPropertyReferenceValue allows a property value to be given by referencing other entities within the resource definitions of IFC.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcPropertyReferenceValue allows a property value to be given by referencing other entities within the resource definitions of IFC. Those other entities are regarded as predefined complex properties and can be aggregated within a property set (IfcPropertySet). The allowable entities to be used as value references are given by the IfcObjectReferenceSelect.
    ///   HISTORY: New entity in IFC Release 1.5. Entity has been renamed from IfcObjectReference in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPropertyReferenceValue : IfcSimpleProperty
    {
        private IfcLabel? _usageName;
        private IfcObjectReferenceSelect _propertyReference;

        #region Constructors

        public IfcPropertyReferenceValue()
        {

        }

        public IfcPropertyReferenceValue(IfcIdentifier name)
            : base(name)
        {
        }

        #endregion

        /// <summary>
        ///   Optional. Description of the use of the referenced value within the property.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Optional)]
        public IfcLabel? UsageName
        {
            get { return _usageName; }
            set { this.SetModelValue(this, ref _usageName, value, v => UsageName = v, "UsageName"); }
        }

        /// <summary>
        ///   Reference to another entity througH one of the select types in IfcObjectReferenceSelect.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory, IfcAttributeType.Class, 1)]
        public IfcObjectReferenceSelect PropertyReference
        {
            get { return _propertyReference; }
            set
            {
                this.SetModelValue(this, ref _propertyReference, value, v => PropertyReference = v,
                                           "PropertyReference");
            }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _usageName = value.StringVal;
                    break;
                case 3:
                    _propertyReference = (IfcObjectReferenceSelect) value.EntityVal;
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
