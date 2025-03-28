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
	public partial class @IfcMember : IIfcMember
	{

		[CrossSchemaAttribute(typeof(IIfcMember), 9)]
		Ifc4.Interfaces.IfcMemberTypeEnum? IIfcMember.PredefinedType 
		{ 
			get
			{
				//## Custom code to handle enumeration of PredefinedType
				//##
				switch (PredefinedType)
				{
					case IfcMemberTypeEnum.ARCH_SEGMENT:
						//## Handle translation of ARCH_SEGMENT member from IfcMemberTypeEnum in property PredefinedType
						return this.GetUserDefined<Ifc4.Interfaces.IfcMemberTypeEnum>();
						//##
					case IfcMemberTypeEnum.BRACE:
						return Ifc4.Interfaces.IfcMemberTypeEnum.BRACE;
					case IfcMemberTypeEnum.CHORD:
						return Ifc4.Interfaces.IfcMemberTypeEnum.CHORD;
					case IfcMemberTypeEnum.COLLAR:
						return Ifc4.Interfaces.IfcMemberTypeEnum.COLLAR;
					case IfcMemberTypeEnum.MEMBER:
						return Ifc4.Interfaces.IfcMemberTypeEnum.MEMBER;
					case IfcMemberTypeEnum.MULLION:
						return Ifc4.Interfaces.IfcMemberTypeEnum.MULLION;
					case IfcMemberTypeEnum.PLATE:
						return Ifc4.Interfaces.IfcMemberTypeEnum.PLATE;
					case IfcMemberTypeEnum.POST:
						return Ifc4.Interfaces.IfcMemberTypeEnum.POST;
					case IfcMemberTypeEnum.PURLIN:
						return Ifc4.Interfaces.IfcMemberTypeEnum.PURLIN;
					case IfcMemberTypeEnum.RAFTER:
						return Ifc4.Interfaces.IfcMemberTypeEnum.RAFTER;
					case IfcMemberTypeEnum.STAY_CABLE:
						//## Handle translation of STAY_CABLE member from IfcMemberTypeEnum in property PredefinedType
						return this.GetUserDefined<Ifc4.Interfaces.IfcMemberTypeEnum>();
						//##
					case IfcMemberTypeEnum.STIFFENING_RIB:
						//## Handle translation of STIFFENING_RIB member from IfcMemberTypeEnum in property PredefinedType
						return this.GetUserDefined<Ifc4.Interfaces.IfcMemberTypeEnum>();
						//##
					case IfcMemberTypeEnum.STRINGER:
						return Ifc4.Interfaces.IfcMemberTypeEnum.STRINGER;
					case IfcMemberTypeEnum.STRUCTURALCABLE:
						//## Handle translation of STRUCTURALCABLE member from IfcMemberTypeEnum in property PredefinedType
						return this.GetUserDefined<Ifc4.Interfaces.IfcMemberTypeEnum>();
						//##
					case IfcMemberTypeEnum.STRUT:
						return Ifc4.Interfaces.IfcMemberTypeEnum.STRUT;
					case IfcMemberTypeEnum.STUD:
						return Ifc4.Interfaces.IfcMemberTypeEnum.STUD;
					case IfcMemberTypeEnum.SUSPENDER:
						//## Handle translation of SUSPENDER member from IfcMemberTypeEnum in property PredefinedType
						return this.GetUserDefined<Ifc4.Interfaces.IfcMemberTypeEnum>();
						//##
					case IfcMemberTypeEnum.SUSPENSION_CABLE:
						//## Handle translation of SUSPENSION_CABLE member from IfcMemberTypeEnum in property PredefinedType
						return this.GetUserDefined<Ifc4.Interfaces.IfcMemberTypeEnum>();
						//##
					case IfcMemberTypeEnum.TIEBAR:
						//## Handle translation of TIEBAR member from IfcMemberTypeEnum in property PredefinedType
						return this.GetUserDefined<Ifc4.Interfaces.IfcMemberTypeEnum>();
						//##
					case IfcMemberTypeEnum.USERDEFINED:
						//## Optional custom handling of PredefinedType == .USERDEFINED. 
						//##
						return Ifc4.Interfaces.IfcMemberTypeEnum.USERDEFINED;
					case IfcMemberTypeEnum.NOTDEFINED:
						return Ifc4.Interfaces.IfcMemberTypeEnum.NOTDEFINED;
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
					case Ifc4.Interfaces.IfcMemberTypeEnum.BRACE:
						PredefinedType = IfcMemberTypeEnum.BRACE;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.CHORD:
						PredefinedType = IfcMemberTypeEnum.CHORD;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.COLLAR:
						PredefinedType = IfcMemberTypeEnum.COLLAR;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.MEMBER:
						PredefinedType = IfcMemberTypeEnum.MEMBER;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.MULLION:
						PredefinedType = IfcMemberTypeEnum.MULLION;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.PLATE:
						PredefinedType = IfcMemberTypeEnum.PLATE;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.POST:
						PredefinedType = IfcMemberTypeEnum.POST;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.PURLIN:
						PredefinedType = IfcMemberTypeEnum.PURLIN;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.RAFTER:
						PredefinedType = IfcMemberTypeEnum.RAFTER;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.STRINGER:
						PredefinedType = IfcMemberTypeEnum.STRINGER;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.STRUT:
						PredefinedType = IfcMemberTypeEnum.STRUT;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.STUD:
						PredefinedType = IfcMemberTypeEnum.STUD;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.USERDEFINED:
						PredefinedType = IfcMemberTypeEnum.USERDEFINED;
						return;
					case Ifc4.Interfaces.IfcMemberTypeEnum.NOTDEFINED:
						PredefinedType = IfcMemberTypeEnum.NOTDEFINED;
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