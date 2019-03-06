using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Xbim.IfcRail.Kernel
{
    public partial struct IfcPropertySetDefinitionSet
    {
        public IEnumerable<IIfcPropertySetDefinition> PropertySetDefinitions 
        { 
            get
            {
                foreach (var pset in this.PropertySetDefinitions)
                {
                    yield return pset;
                }
            }
        }
    }
}
