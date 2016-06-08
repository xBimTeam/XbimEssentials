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

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
	public partial class @IfcDistributionPort : IIfcDistributionPort
	{
		Ifc4.Interfaces.IfcFlowDirectionEnum? IIfcDistributionPort.FlowDirection 
		{ 
			get
			{
				switch (FlowDirection)
				{
					case IfcFlowDirectionEnum.SOURCE:
						return Ifc4.Interfaces.IfcFlowDirectionEnum.SOURCE;
					
					case IfcFlowDirectionEnum.SINK:
						return Ifc4.Interfaces.IfcFlowDirectionEnum.SINK;
					
					case IfcFlowDirectionEnum.SOURCEANDSINK:
						return Ifc4.Interfaces.IfcFlowDirectionEnum.SOURCEANDSINK;
					
					case IfcFlowDirectionEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcFlowDirectionEnum.NOTDEFINED;
					
					
					default:
						throw new System.ArgumentOutOfRangeException();
				}
			} 
			set
			{
				switch (value)
				{
					case Ifc4.Interfaces.IfcFlowDirectionEnum.SOURCE:
						FlowDirection = IfcFlowDirectionEnum.SOURCE;
						return;
					
					case Ifc4.Interfaces.IfcFlowDirectionEnum.SINK:
						FlowDirection = IfcFlowDirectionEnum.SINK;
						return;
					
					case Ifc4.Interfaces.IfcFlowDirectionEnum.SOURCEANDSINK:
						FlowDirection = IfcFlowDirectionEnum.SOURCEANDSINK;
						return;
					
					case Ifc4.Interfaces.IfcFlowDirectionEnum.NOTDEFINED:
						FlowDirection = IfcFlowDirectionEnum.NOTDEFINED;
						return;
					
					
					default:
						throw new System.ArgumentOutOfRangeException();
				}
				
			}
		}

		private  Ifc4.Interfaces.IfcDistributionPortTypeEnum? _predefinedType;

		Ifc4.Interfaces.IfcDistributionPortTypeEnum? IIfcDistributionPort.PredefinedType 
		{ 
			get
			{
				return _predefinedType;
			} 
			set
			{
				SetValue(v => _predefinedType = v, _predefinedType, value, "PredefinedType", byte.MaxValue);
				
			}
		}

		private  Ifc4.Interfaces.IfcDistributionSystemEnum? _systemType;

		Ifc4.Interfaces.IfcDistributionSystemEnum? IIfcDistributionPort.SystemType 
		{ 
			get
			{
				return _systemType;
			} 
			set
			{
				SetValue(v => _systemType = v, _systemType, value, "SystemType", byte.MaxValue);
				
			}
		}
	//## Custom code
	//##
	}
}