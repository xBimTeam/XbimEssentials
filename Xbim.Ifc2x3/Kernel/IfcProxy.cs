#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProxy.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   The IfcProxy is intended to be a kind of a container for wrapping objects which are defined by associated properties, which may or may not have a geometric representation and placement in space.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcProxy is intended to be a kind of a container for wrapping objects which are defined by associated properties, which may or may not have a geometric representation and placement in space. A proxy may have a semantic meaning, defined by the Name attribute, and property definitions, attached through the property assignment relationship, which definition may be outside of the definitions given by the current release of IFC.
    ///   The ProxyType may give an indication to which high level semantic breakdown of object the semantic definition of the proxy relates to. the Tag attribute may be used to assign a human or system interpretable identifier (such as a serial number or bar code).
    ///   NOTE 1  Given that only a limited number of semantic constructs can be formally defined within IFC (and it will never be possible to define all), there has to be a mechanism for capturing those constructs that are not (yet) defined by IFC. 
    ///   NOTE 2  Product proxies are a mechanism that allows to exchange data that is part of the project but not necessarily part of the IFC model. Those proxies may have geometric representations assigned.
    ///   HISTORY  New entity in IFC Release 1.5.
    ///   Formal Propositions:
    ///   WR1   :   The Name attribute has to be provided for a proxy.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcProxy : IfcProduct
    {
        private IfcObjectType _proxyType;
        private IfcLabel _tag;

        /// <summary>
        ///   High level (and only) semantic meaning attached to the IfcProxy, defining the basic construct type behind the Proxy, e.g. Product or Process.
        /// </summary>
        public IfcObjectType ProxyType
        {
            get { return _proxyType; }
            set { _proxyType = value; }
        }

        /// <summary>
        ///   The tag (or label) identifier at the particular instance of a product, e.g. the serial number, or the position number. It is the identifier at the occurrance level.
        /// </summary>
        public IfcLabel Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }
}