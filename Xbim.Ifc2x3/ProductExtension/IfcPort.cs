#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPort.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   An IfcPort provides the means for an element to connect to other elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: An IfcPort provides the means for an element to connect to other elements.
    ///   An IfcPort is associated with an IfcElement, it belongs to, through the objectified relationship IfcRelConnectsPortToElement. Exactly two ports, belonging to two different elements, are connected with each other through the objectified relationship IfcRelConnectsPorts.
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    ///   Use Definitions
    ///   An instance of IfcElement may have one or more points at which it connects to other instances of IfcElement. An instance of IfcPort is located at a point where a connection can occur. The location of the port is determined in the context of the local coordinate system of the element to which it belongs.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcPort : IfcProduct
    {
        /// <summary>
        ///   Inverse. Reference to the element to port connection relationship. The relationship then refers to the element in which this port is contained.
        /// </summary>
        public IfcRelConnectsPortToElement ContainedIn
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        ///   Inverse. Reference to a port that is connected by the objectified relationship.
        /// </summary>
        public IfcRelConnectsPorts ConnectedFrom
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        ///   Inverse. Reference to the port connection relationship. The relationship then refers to the other port to which this port is connected.
        /// </summary>
        public IfcRelConnectsPorts ConnectedTo
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}