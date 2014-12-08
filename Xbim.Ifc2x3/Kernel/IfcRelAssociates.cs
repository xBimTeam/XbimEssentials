#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssociates.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    [IfcPersistedEntityAttribute]
    public abstract class IfcRelAssociates : IfcRelationship
    {
        public IfcRelAssociates()
        {
            _relatedObjects = new XbimSet<IfcRoot>(this);
        }

        #region Fields and Events

        private XbimSet<IfcRoot> _relatedObjects;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Objects or Types, to which the external references or information is associated
        /// </summary>
        /// <remarks>
        ///   WR21   :   The IfcRelAssociates relationship is restricted to associate information object, such as classification, document, library information, matsel, etc., to semantic object (occurrence objects based on IfcObject, and type objects, based on IfcTypeObject).
        /// </remarks>
        [IfcAttribute(5, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        [IndexedProperty]
        public XbimSet<IfcRoot> RelatedObjects
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
                    _relatedObjects.Add((IfcRoot) value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        public override string WhereRule()
        {
            if (RelatedObjects.Where(r => !(r is IfcObjectDefinition || r is IfcPropertyDefinition)).Count() != 0)
                return
                    "WR21 RelAssociates: The RelAssociates relationship is restricted to associate information object, such as classification, document, library information, material, etc.\n";
            else return "";
        }
    }
}