using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
