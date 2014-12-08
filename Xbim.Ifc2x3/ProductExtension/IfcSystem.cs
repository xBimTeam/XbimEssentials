#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSystem.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    [IfcPersistedEntityAttribute]
    public class IfcSystem : IfcGroup
    {
        /// <summary>
        ///   Inverse. Reference to the spatial structure via the objectified relationship IfcRelServicesBuildings, which is serviced by the system.
        /// </summary>
        public IEnumerable<IfcRelServicesBuildings> ServicesBuildings
        {
            get { return ModelOf.Instances.Where<IfcRelServicesBuildings>(r => r.RelatingSystem == this); }
        }
    }
}