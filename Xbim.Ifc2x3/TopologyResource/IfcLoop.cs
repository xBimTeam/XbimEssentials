#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLoop.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    [IfcPersistedEntityAttribute]
    public class IfcLoop : IfcTopologicalRepresentationItem
    {
        public override void IfcParse(int propIndex, IPropertyValue value)
        {
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}