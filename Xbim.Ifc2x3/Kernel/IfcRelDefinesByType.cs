#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelDefinesByType.cs
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
    ///   This objectified relationship (IfcRelDefinesByType) defines the relationships between an object type and objects.
    ///   The IfcRelDefinesByType is a 1-to-N relationship, as it allows for the assignment of one type information to a single or to many objects. 
    ///   Those objects then share the same object type.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcRelDefinesByType : IfcRelDefines
    {
        #region Fields and Events

        private IfcTypeObject _relatingType;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Reference to the type (or style) information for that object or set of objects.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        [IndexedProperty]
        public IfcTypeObject RelatingType
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingType;
            }
            set { this.SetModelValue(this, ref _relatingType, value, v => RelatingType = v, "RelatingType"); }
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
                    _relatingType = (IfcTypeObject) value.EntityVal;
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