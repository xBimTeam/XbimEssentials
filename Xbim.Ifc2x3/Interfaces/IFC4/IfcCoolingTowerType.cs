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
namespace Xbim.Ifc2x3.HVACDomain
{
	public partial class @IfcCoolingTowerType : IIfcCoolingTowerType
	{
		Ifc4.Interfaces.IfcCoolingTowerTypeEnum IIfcCoolingTowerType.PredefinedType 
		{ 
			get
			{
				switch (PredefinedType)
				{
					case IfcCoolingTowerTypeEnum.NATURALDRAFT:
						return Ifc4.Interfaces.IfcCoolingTowerTypeEnum.NATURALDRAFT;
					
					case IfcCoolingTowerTypeEnum.MECHANICALINDUCEDDRAFT:
						return Ifc4.Interfaces.IfcCoolingTowerTypeEnum.MECHANICALINDUCEDDRAFT;
					
					case IfcCoolingTowerTypeEnum.MECHANICALFORCEDDRAFT:
						return Ifc4.Interfaces.IfcCoolingTowerTypeEnum.MECHANICALFORCEDDRAFT;
					
					case IfcCoolingTowerTypeEnum.USERDEFINED:
						//## Optional custom handling of PredefinedType == .USERDEFINED. 
						//##
						return Ifc4.Interfaces.IfcCoolingTowerTypeEnum.USERDEFINED;
					
					case IfcCoolingTowerTypeEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcCoolingTowerTypeEnum.NOTDEFINED;
					
					
					default:
						throw new System.ArgumentOutOfRangeException();
				}
			} 
			set
			{
				switch (value)
				{
					case Ifc4.Interfaces.IfcCoolingTowerTypeEnum.NATURALDRAFT:
						PredefinedType = IfcCoolingTowerTypeEnum.NATURALDRAFT;
						return;
					
					case Ifc4.Interfaces.IfcCoolingTowerTypeEnum.MECHANICALINDUCEDDRAFT:
						PredefinedType = IfcCoolingTowerTypeEnum.MECHANICALINDUCEDDRAFT;
						return;
					
					case Ifc4.Interfaces.IfcCoolingTowerTypeEnum.MECHANICALFORCEDDRAFT:
						PredefinedType = IfcCoolingTowerTypeEnum.MECHANICALFORCEDDRAFT;
						return;
					
					case Ifc4.Interfaces.IfcCoolingTowerTypeEnum.USERDEFINED:
						PredefinedType = IfcCoolingTowerTypeEnum.USERDEFINED;
						return;
					
					case Ifc4.Interfaces.IfcCoolingTowerTypeEnum.NOTDEFINED:
						PredefinedType = IfcCoolingTowerTypeEnum.NOTDEFINED;
						return;
					
					
					default:
						throw new System.ArgumentOutOfRangeException();
				}
				
			}
		}
	//## Custom code
	//##
	}
}