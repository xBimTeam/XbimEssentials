#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectsWithRealizingElements.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   IfcRelConnectsWithRealizingElements defines a generic relationship that is made between two elements that require the realization of that relationship by means of further realizing elements.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: IfcRelConnectsWithRealizingElements defines a generic relationship that is made between two elements that require the realization of that relationship by means of further realizing elements.
    ///   An IfcRelConnectsWithRealizingElements is a specialization of IfcRelConnectsElement where the connecting operation has the additional attribute of (one or many) realizing elements that may be used to realize or further qualify the relationship. It is defined as a ternary relationship. 
    ///   EXAMPLE: It may be used to describe the attachment of one element to another where the attachment is realized by a 'fixing' element such as a bracket. It may also be used to describe the mounting of one element onto another such as the requirement for the mounting major plant items onto builders work bases and/or anti-vibration isolators.
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelConnectsWithRealizingElements : IfcRelConnectsElements
    {
        /// <summary>
        ///   Defines the elements that realize a connection relationship.
        /// </summary>
        public XbimList<IfcElement> RealizingElements
        {
            get { throw new NotImplementedException(); }
            set { }
        }

        /// <summary>
        ///   The type of the connection given for informal purposes, it may include labels, like 'joint', 'rigid joint', 'flexible joint', etc.
        /// </summary>
        public IfcLabel ConnectionType
        {
            get { throw new NotImplementedException(); }
            set { }
        }
    }
}