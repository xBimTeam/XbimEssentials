using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.Interfaces;

namespace Xbim.IfcRail.Kernel
{
    public abstract partial class IfcPropertySetDefinition 
    {
        public IEnumerable<IIfcPropertySetDefinition> PropertySetDefinitions
        {
            get
            {
                return new IIfcPropertySetDefinition[] { this };
            }
        }
    }
}
