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
namespace Xbim.Ifc4x3.HvacDomain
{
	public partial class @IfcVibrationIsolatorType : IIfcVibrationIsolatorType
	{

		[CrossSchemaAttribute(typeof(IIfcVibrationIsolatorType), 10)]
		Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum IIfcVibrationIsolatorType.PredefinedType 
		{ 
			get
			{
				//## Custom code to handle enumeration of PredefinedType
				//##
				switch (PredefinedType)
				{
					case IfcVibrationIsolatorTypeEnum.BASE:
						//## Handle translation of BASE member from IfcVibrationIsolatorTypeEnum in property PredefinedType
						return this.GetUserDefined<Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum>();
						//##
					case IfcVibrationIsolatorTypeEnum.COMPRESSION:
						return Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum.COMPRESSION;
					case IfcVibrationIsolatorTypeEnum.SPRING:
						return Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum.SPRING;
					case IfcVibrationIsolatorTypeEnum.USERDEFINED:
						//## Optional custom handling of PredefinedType == .USERDEFINED. 
						//##
						return Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum.USERDEFINED;
					case IfcVibrationIsolatorTypeEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum.NOTDEFINED;
					
					default:
						throw new System.ArgumentOutOfRangeException();
				}
			} 
			set
			{
				//## Custom code to handle setting of enumeration of PredefinedType
				//##
				switch (value)
				{
					case Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum.COMPRESSION:
						PredefinedType = IfcVibrationIsolatorTypeEnum.COMPRESSION;
						return;
					case Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum.SPRING:
						PredefinedType = IfcVibrationIsolatorTypeEnum.SPRING;
						return;
					case Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum.USERDEFINED:
						PredefinedType = IfcVibrationIsolatorTypeEnum.USERDEFINED;
						return;
					case Ifc4.Interfaces.IfcVibrationIsolatorTypeEnum.NOTDEFINED:
						PredefinedType = IfcVibrationIsolatorTypeEnum.NOTDEFINED;
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