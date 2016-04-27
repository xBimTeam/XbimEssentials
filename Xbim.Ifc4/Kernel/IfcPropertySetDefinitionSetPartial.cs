using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4.Kernel
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
