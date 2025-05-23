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
namespace Xbim.Ifc4x3.SharedBldgElements
{
	public partial class @IfcColumn : IIfcColumn
	{

		[CrossSchemaAttribute(typeof(IIfcColumn), 9)]
		Ifc4.Interfaces.IfcColumnTypeEnum? IIfcColumn.PredefinedType 
		{ 
			get
			{
				//## Custom code to handle enumeration of PredefinedType
				//##
				switch (PredefinedType)
				{
					case IfcColumnTypeEnum.COLUMN:
						return Ifc4.Interfaces.IfcColumnTypeEnum.COLUMN;
					case IfcColumnTypeEnum.PIERSTEM:
                        //## Handle translation of PIERSTEM member from IfcColumnTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcColumnTypeEnum>();
                    //##
                    case IfcColumnTypeEnum.PIERSTEM_SEGMENT:
                        //## Handle translation of PIERSTEM_SEGMENT member from IfcColumnTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcColumnTypeEnum>();
                    //##
                    case IfcColumnTypeEnum.PILASTER:
						return Ifc4.Interfaces.IfcColumnTypeEnum.PILASTER;
					case IfcColumnTypeEnum.STANDCOLUMN:
                        //## Handle translation of STANDCOLUMN member from IfcColumnTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcColumnTypeEnum>();
                    //##
                    case IfcColumnTypeEnum.USERDEFINED:
						//## Optional custom handling of PredefinedType == .USERDEFINED. 
						//##
						return Ifc4.Interfaces.IfcColumnTypeEnum.USERDEFINED;
					case IfcColumnTypeEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcColumnTypeEnum.NOTDEFINED;
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
					case Ifc4.Interfaces.IfcColumnTypeEnum.COLUMN:
						PredefinedType = IfcColumnTypeEnum.COLUMN;
						return;
					case Ifc4.Interfaces.IfcColumnTypeEnum.PILASTER:
						PredefinedType = IfcColumnTypeEnum.PILASTER;
						return;
					case Ifc4.Interfaces.IfcColumnTypeEnum.USERDEFINED:
						PredefinedType = IfcColumnTypeEnum.USERDEFINED;
						return;
					case Ifc4.Interfaces.IfcColumnTypeEnum.NOTDEFINED:
						PredefinedType = IfcColumnTypeEnum.NOTDEFINED;
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