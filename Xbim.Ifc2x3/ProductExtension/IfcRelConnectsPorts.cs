#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsPorts.cs
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
    ///   An IfcRelConnectsPorts defines the relationship that is made between two ports at their point of connection. It may include the connection geometry between two ports.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcRelConnectsPorts defines the relationship that is made between two ports at their point of connection. It may include the connection geometry between two ports.
    ///   IfcRelConnectsPorts is required as the means to define how instances of IfcPort connect together. Each of the port is being attached to the IfcElement by using the IfcRelConnectsPortToElement relationship. Since each element is now considered to have ports for the purpose of connection, it is necessary to achieve the actual connection by the defined approach.
    ///   HISTORY: New entity in IFC 2.0, modified in IFC2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsPorts : IfcRelConnects
    {
        #region Fields

        private IfcPort _relatingPort;
        private IfcPort _relatedPort;
        private IfcElement _realizingElement;

        #endregion

        /// <summary>
        ///   Reference to the first port that is connected by the objectified relationship.
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
        ///   Reference to the second port that is connected by the objectified relationship.
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory), IndexedProperty]
        public IfcPort RelatedPort
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _relatedPort;
            }
            set { this.SetModelValue(this, ref _relatedPort, value, v => RelatedPort = v, "RelatedPort"); }
        }

        /// <summary>
        ///   Optional. Defines the element that realizes a port connection relationship.
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Optional)]
        public IfcElement RealizingElement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _realizingElement;
            }
            set
            {
                this.SetModelValue(this, ref _realizingElement, value, v => RealizingElement = v,
                                           "RealizingElement");
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
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _relatingPort = (IfcPort) value.EntityVal;
                    break;
                case 5:
                    _relatedPort = (IfcPort) value.EntityVal;
                    break;
                case 6:
                    _realizingElement = (IfcElement) value.EntityVal;
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