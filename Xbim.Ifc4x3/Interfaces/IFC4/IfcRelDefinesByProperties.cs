// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;

//## Custom using statements
//##

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc4x3.Kernel
{
	public partial class @IfcRelDefinesByProperties : IIfcRelDefinesByProperties
	{

		[CrossSchemaAttribute(typeof(IIfcRelDefinesByProperties), 5)]
		IItemSet<IIfcObjectDefinition> IIfcRelDefinesByProperties.RelatedObjects 
		{ 
			get
			{
			
				return new Common.Collections.ProxyItemSet<IfcObjectDefinition, IIfcObjectDefinition>(RelatedObjects);
			} 
		}

		[CrossSchemaAttribute(typeof(IIfcRelDefinesByProperties), 6)]
		IIfcPropertySetDefinitionSelect IIfcRelDefinesByProperties.RelatingPropertyDefinition 
		{ 
			get
			{
				if (RelatingPropertyDefinition == null) return null;
				var ifcpropertysetdefinition = RelatingPropertyDefinition as IfcPropertySetDefinition;
				if (ifcpropertysetdefinition != null) 
					return ifcpropertysetdefinition;
				//## Handle defined type IfcPropertySetDefinitionSet which is not a part of the target select interface IIfcPropertySetDefinitionSelect in property RelatingPropertyDefinition
				if (RelatingPropertyDefinition is IfcPropertySetDefinitionSet set)
					return set;
				//##
				return null;
			} 
			set
			{
				if (value == null)
				{
					RelatingPropertyDefinition = null;
					return;
				}	
				var ifcpropertysetdefinition = value as IfcPropertySetDefinition;
				if (ifcpropertysetdefinition != null) 
				{
					RelatingPropertyDefinition = ifcpropertysetdefinition;
					return;
				}
				if (value is Ifc4.Kernel.IfcPropertySetDefinitionSet) 
				{
					//## Handle setting of defined type IfcPropertySetDefinitionSet which is not a part of the target select interface IIfcPropertySetDefinitionSelect in property RelatingPropertyDefinition
					//TODO: Handle setting of defined type IfcPropertySetDefinitionSet which is not a part of the target select interface IIfcPropertySetDefinitionSelect in property RelatingPropertyDefinition
					throw new System.NotImplementedException();
					//##
				}
				
			}
		}
	//## Custom code
	//##
	}
}