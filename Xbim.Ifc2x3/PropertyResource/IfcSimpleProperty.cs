#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSimpleProperty.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.PropertyResource
{
    /// <summary>
    ///   A generalization of a single property object. The various subtypes of IfcSimpleProperty establish different ways in which a property value can be set.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public abstract class IfcSimpleProperty : IfcProperty
    {
        #region Constructors

        public IfcSimpleProperty(IfcIdentifier name)
            : base(name)
        {
        }

        public IfcSimpleProperty()
        {
        }

        #endregion
    }
}