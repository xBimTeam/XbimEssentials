using System.Collections.Generic;

namespace Xbim.Ifc4x3.Kernel
{
	public partial struct IfcPropertySetDefinitionSet
	{
		public IEnumerable<IfcPropertySetDefinition> PropertySetDefinitions => _value;
		
	}
}
