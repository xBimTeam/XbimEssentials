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
namespace Xbim.Ifc2x3.ElectricalDomain
{
	public partial class @IfcElectricGeneratorType : IIfcElectricGeneratorType
	{
		Ifc4.Interfaces.IfcElectricGeneratorTypeEnum IIfcElectricGeneratorType.PredefinedType 
		{ 
			get
			{
				switch (PredefinedType)
				{
					case IfcElectricGeneratorTypeEnum.USERDEFINED:
						//## Optional custom handling of PredefinedType == .USERDEFINED. 
						//##
						return Ifc4.Interfaces.IfcElectricGeneratorTypeEnum.USERDEFINED;
					
					case IfcElectricGeneratorTypeEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcElectricGeneratorTypeEnum.NOTDEFINED;
					
					
					default:
						throw new System.ArgumentOutOfRangeException();
				}
			} 
			set
			{
				switch (value)
				{
					case Ifc4.Interfaces.IfcElectricGeneratorTypeEnum.CHP:
						//## Handle setting of CHP member from IfcElectricGeneratorTypeEnum in property PredefinedType
						//TODO: Handle setting of CHP member from IfcElectricGeneratorTypeEnum in property PredefinedType
						throw new System.NotImplementedException();
						//##
										
					case Ifc4.Interfaces.IfcElectricGeneratorTypeEnum.ENGINEGENERATOR:
						//## Handle setting of ENGINEGENERATOR member from IfcElectricGeneratorTypeEnum in property PredefinedType
						//TODO: Handle setting of ENGINEGENERATOR member from IfcElectricGeneratorTypeEnum in property PredefinedType
						throw new System.NotImplementedException();
						//##
										
					case Ifc4.Interfaces.IfcElectricGeneratorTypeEnum.STANDALONE:
						//## Handle setting of STANDALONE member from IfcElectricGeneratorTypeEnum in property PredefinedType
						//TODO: Handle setting of STANDALONE member from IfcElectricGeneratorTypeEnum in property PredefinedType
						throw new System.NotImplementedException();
						//##
										
					case Ifc4.Interfaces.IfcElectricGeneratorTypeEnum.USERDEFINED:
						PredefinedType = IfcElectricGeneratorTypeEnum.USERDEFINED;
						return;
					
					case Ifc4.Interfaces.IfcElectricGeneratorTypeEnum.NOTDEFINED:
						PredefinedType = IfcElectricGeneratorTypeEnum.NOTDEFINED;
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