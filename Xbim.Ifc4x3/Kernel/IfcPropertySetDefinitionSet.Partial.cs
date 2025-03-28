using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4x3.Kernel
{
	public partial struct IfcPropertySetDefinitionSet : IIfcPropertySetDefinitionSelect
    {
		public IEnumerable<IfcPropertySetDefinition> PropertySetDefinitions => _value;

        IEnumerable<IIfcPropertySetDefinition> IIfcPropertySetDefinitionSelect.PropertySetDefinitions => _value;
    }
}
