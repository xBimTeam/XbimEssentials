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
namespace Xbim.Ifc2x3.SharedBldgElements
{
	public partial class @IfcSlab : IIfcSlab
	{
		Ifc4.Interfaces.IfcSlabTypeEnum? IIfcSlab.PredefinedType 
		{ 
			get
			{
				switch (PredefinedType)
				{
					case IfcSlabTypeEnum.FLOOR:
						return Ifc4.Interfaces.IfcSlabTypeEnum.FLOOR;
					
					case IfcSlabTypeEnum.ROOF:
						return Ifc4.Interfaces.IfcSlabTypeEnum.ROOF;
					
					case IfcSlabTypeEnum.LANDING:
						return Ifc4.Interfaces.IfcSlabTypeEnum.LANDING;
					
					case IfcSlabTypeEnum.BASESLAB:
						return Ifc4.Interfaces.IfcSlabTypeEnum.BASESLAB;
					
					case IfcSlabTypeEnum.USERDEFINED:
						//## Optional custom handling of PredefinedType == .USERDEFINED. 
						//##
						return Ifc4.Interfaces.IfcSlabTypeEnum.USERDEFINED;
					
					case IfcSlabTypeEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcSlabTypeEnum.NOTDEFINED;
					
					
					default:
						throw new System.ArgumentOutOfRangeException();
				}
			} 
			set
			{
				switch (value)
				{
					case Ifc4.Interfaces.IfcSlabTypeEnum.FLOOR:
						PredefinedType = IfcSlabTypeEnum.FLOOR;
						return;
					
					case Ifc4.Interfaces.IfcSlabTypeEnum.ROOF:
						PredefinedType = IfcSlabTypeEnum.ROOF;
						return;
					
					case Ifc4.Interfaces.IfcSlabTypeEnum.LANDING:
						PredefinedType = IfcSlabTypeEnum.LANDING;
						return;
					
					case Ifc4.Interfaces.IfcSlabTypeEnum.BASESLAB:
						PredefinedType = IfcSlabTypeEnum.BASESLAB;
						return;
					
					case Ifc4.Interfaces.IfcSlabTypeEnum.USERDEFINED:
						PredefinedType = IfcSlabTypeEnum.USERDEFINED;
						return;
					
					case Ifc4.Interfaces.IfcSlabTypeEnum.NOTDEFINED:
						PredefinedType = IfcSlabTypeEnum.NOTDEFINED;
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