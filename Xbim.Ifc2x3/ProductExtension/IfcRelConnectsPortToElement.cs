#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsPortToElement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   An IfcRelConnectsPortToElement defines the relationship that is made between one port to the IfcElement in which it is contained.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsPortToElement : IfcRelConnects
    {
        #region Fields

        private IfcPort _relatingPort;
        private IfcElement _relatedElement;

        #endregion

        /// <summary>
        ///   Reference to an Port that is connected by the objectified relationship.
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcPort RelatingPort
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatingPort;
            }
            set { this.SetModelValue(this, ref _relatingPort, value, v => RelatingPort = v, "RelatingPort"); }
        }

        /// <summary>
        ///   Reference to an Element that is connected by the objectified relationship.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcElement RelatedElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedElement;
            }
            set { this.SetModelValue(this, ref _relatedElement, value, v => RelatedElement = v, "RelatedElement"); }
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
                    _relatingPort = (IfcPort) value.EntityVal;
                    break;
                case 5:
                    _relatedElement = (IfcElement) value.EntityVal;
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