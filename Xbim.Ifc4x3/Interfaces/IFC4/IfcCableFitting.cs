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
namespace Xbim.Ifc4x3.ElectricalDomain
{
	public partial class @IfcCableFitting : IIfcCableFitting
	{

		[CrossSchemaAttribute(typeof(IIfcCableFitting), 9)]
		Ifc4.Interfaces.IfcCableFittingTypeEnum? IIfcCableFitting.PredefinedType 
		{ 
			get
			{
				//## Custom code to handle enumeration of PredefinedType
				//##
				switch (PredefinedType)
				{
					case IfcCableFittingTypeEnum.CONNECTOR:
						return Ifc4.Interfaces.IfcCableFittingTypeEnum.CONNECTOR;
					case IfcCableFittingTypeEnum.ENTRY:
						return Ifc4.Interfaces.IfcCableFittingTypeEnum.ENTRY;
					case IfcCableFittingTypeEnum.EXIT:
						return Ifc4.Interfaces.IfcCableFittingTypeEnum.EXIT;
					case IfcCableFittingTypeEnum.FANOUT:
                        //## Handle translation of FANOUT member from IfcCableFittingTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCableFittingTypeEnum>();
                    //##
                    case IfcCableFittingTypeEnum.JUNCTION:
						return Ifc4.Interfaces.IfcCableFittingTypeEnum.JUNCTION;
					case IfcCableFittingTypeEnum.TRANSITION:
						return Ifc4.Interfaces.IfcCableFittingTypeEnum.TRANSITION;
					case IfcCableFittingTypeEnum.USERDEFINED:
						//## Optional custom handling of PredefinedType == .USERDEFINED. 
						//##
						return Ifc4.Interfaces.IfcCableFittingTypeEnum.USERDEFINED;
					case IfcCableFittingTypeEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcCableFittingTypeEnum.NOTDEFINED;
					case null: 
						return null;
					
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
					case Ifc4.Interfaces.IfcCableFittingTypeEnum.CONNECTOR:
						PredefinedType = IfcCableFittingTypeEnum.CONNECTOR;
						return;
					case Ifc4.Interfaces.IfcCableFittingTypeEnum.ENTRY:
						PredefinedType = IfcCableFittingTypeEnum.ENTRY;
						return;
					case Ifc4.Interfaces.IfcCableFittingTypeEnum.EXIT:
						PredefinedType = IfcCableFittingTypeEnum.EXIT;
						return;
					case Ifc4.Interfaces.IfcCableFittingTypeEnum.JUNCTION:
						PredefinedType = IfcCableFittingTypeEnum.JUNCTION;
						return;
					case Ifc4.Interfaces.IfcCableFittingTypeEnum.TRANSITION:
						PredefinedType = IfcCableFittingTypeEnum.TRANSITION;
						return;
					case Ifc4.Interfaces.IfcCableFittingTypeEnum.USERDEFINED:
						PredefinedType = IfcCableFittingTypeEnum.USERDEFINED;
						return;
					case Ifc4.Interfaces.IfcCableFittingTypeEnum.NOTDEFINED:
						PredefinedType = IfcCableFittingTypeEnum.NOTDEFINED;
						return;
					
					case null:
						PredefinedType = null;
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