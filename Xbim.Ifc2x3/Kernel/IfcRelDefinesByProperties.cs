#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelDefinesByProperties.cs
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
    ///   This objectified relationship (IfcRelDefinesByProperties) defines the relationships between property set definitions and objects. 
    ///   Properties are aggregated in property sets, property sets can be grouped to define an object type.
    ///   The IfcRelDefinesByProperties is a 1-to-N relationship, as it allows for the assignment of one property set to a single or to many objects. 
    ///   Those objects then share the same property definition.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcRelDefinesByProperties : IfcRelDefines
    {
        #region Fields

        private IfcPropertySetDefinition _relatingPropertyDefinition;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   Reference to the property set definition for that object or set of objects.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        [IndexedProperty]
        public IfcPropertySetDefinition RelatingPropertyDefinition
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingPropertyDefinition;
            }
            set
            {
                this.SetModelValue(this, ref _relatingPropertyDefinition, value,
                                           v => RelatingPropertyDefinition = v, "RelatingPropertyDefinition");
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
                    _relatingPropertyDefinition = value.EntityVal as IfcPropertySetDefinition;
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