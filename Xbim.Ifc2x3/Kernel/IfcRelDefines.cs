#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelDefines.cs
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
    ///   A definition relationship (IfcRelDefines) that uses a type definition or property set definition (seens as partial type information) to define the properties of the object instance.
    ///   It is a specific - occurrence relationship with implied dependencies (as the occurrence properties depend on the specific properties).
    ///   The IfcRelDefines relationship establishes the link between one type (specific) information and several objects (occurrences). 
    ///   Those occurrences then share the same type (or partial type) information.
    ///   EXAMPLE: Several instances of windows within the IFC project model may be of the same (catalogue or manufacturer) type. Thereby they share the same properties. 
    ///   This relationship is established by a subtype of the IfcRelDefines relationship assigning an IfcProductType (or subtype thereof) to the IfcWindow.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public abstract class IfcRelDefines : IfcRelationship
    {
        public IfcRelDefines()
        {
            _relatedObjects = new IfcObjectSet(this);
        }

        #region Fields and Events

        private IfcObjectSet _relatedObjects;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Reference to the objects (or single object) to which the property definition applies.
        /// </summary>
        [IndexedProperty]
        [IfcAttribute(5, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public IfcObjectSet RelatedObjects
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedObjects;
            }
            set { this.SetModelValue(this, ref _relatedObjects, value, v => RelatedObjects = v, "RelatedObjects"); }
        }

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
                    _relatedObjects.Add((IfcObject) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region Ifc Schema Validation Methods

        #endregion
    }
}
