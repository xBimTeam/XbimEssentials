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
	public partial class @IfcCommunicationsApplianceType : IIfcCommunicationsApplianceType
	{

		[CrossSchemaAttribute(typeof(IIfcCommunicationsApplianceType), 10)]
		Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum IIfcCommunicationsApplianceType.PredefinedType 
		{ 
			get
			{
				//## Custom code to handle enumeration of PredefinedType
				//##
				switch (PredefinedType)
				{
					case IfcCommunicationsApplianceTypeEnum.ANTENNA:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.ANTENNA;
					case IfcCommunicationsApplianceTypeEnum.AUTOMATON:
                        //## Handle translation of AUTOMATON member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.COMPUTER:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.COMPUTER;
					case IfcCommunicationsApplianceTypeEnum.FAX:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.FAX;
					case IfcCommunicationsApplianceTypeEnum.GATEWAY:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.GATEWAY;
					case IfcCommunicationsApplianceTypeEnum.INTELLIGENTPERIPHERAL:
                        //## Handle translation of INTELLIGENTPERIPHERAL member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.IPNETWORKEQUIPMENT:
                        //## Handle translation of IPNETWORKEQUIPMENT member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.LINESIDEELECTRONICUNIT:
                        //## Handle translation of LINESIDEELECTRONICUNIT member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.MODEM:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.MODEM;
					case IfcCommunicationsApplianceTypeEnum.NETWORKAPPLIANCE:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.NETWORKAPPLIANCE;
					case IfcCommunicationsApplianceTypeEnum.NETWORKBRIDGE:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.NETWORKBRIDGE;
					case IfcCommunicationsApplianceTypeEnum.NETWORKHUB:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.NETWORKHUB;
					case IfcCommunicationsApplianceTypeEnum.OPTICALLINETERMINAL:
                        //## Handle translation of OPTICALLINETERMINAL member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.OPTICALNETWORKUNIT:
                        //## Handle translation of OPTICALNETWORKUNIT member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.PRINTER:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.PRINTER;
					case IfcCommunicationsApplianceTypeEnum.RADIOBLOCKCENTER:
                        //## Handle translation of RADIOBLOCKCENTER member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.REPEATER:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.REPEATER;
					case IfcCommunicationsApplianceTypeEnum.ROUTER:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.ROUTER;
					case IfcCommunicationsApplianceTypeEnum.SCANNER:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.SCANNER;
					case IfcCommunicationsApplianceTypeEnum.TELECOMMAND:
                        //## Handle translation of TELECOMMAND member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.TELEPHONYEXCHANGE:
                        //## Handle translation of TELEPHONYEXCHANGE member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.TRANSITIONCOMPONENT:
                        //## Handle translation of TRANSITIONCOMPONENT member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.TRANSPONDER:
                        //## Handle translation of TRANSPONDER member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.TRANSPORTEQUIPMENT:
                        //## Handle translation of TRANSPORTEQUIPMENT member from IfcCommunicationsApplianceTypeEnum in property PredefinedType
                        return this.GetUserDefined<Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum>();
                    //##
                    case IfcCommunicationsApplianceTypeEnum.USERDEFINED:
						//## Optional custom handling of PredefinedType == .USERDEFINED. 
						//##
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.USERDEFINED;
					case IfcCommunicationsApplianceTypeEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.NOTDEFINED;
					
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
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.ANTENNA:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.ANTENNA;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.COMPUTER:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.COMPUTER;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.FAX:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.FAX;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.GATEWAY:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.GATEWAY;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.MODEM:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.MODEM;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.NETWORKAPPLIANCE:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.NETWORKAPPLIANCE;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.NETWORKBRIDGE:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.NETWORKBRIDGE;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.NETWORKHUB:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.NETWORKHUB;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.PRINTER:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.PRINTER;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.REPEATER:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.REPEATER;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.ROUTER:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.ROUTER;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.SCANNER:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.SCANNER;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.USERDEFINED:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.USERDEFINED;
						return;
					case Ifc4.Interfaces.IfcCommunicationsApplianceTypeEnum.NOTDEFINED:
						PredefinedType = IfcCommunicationsApplianceTypeEnum.NOTDEFINED;
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