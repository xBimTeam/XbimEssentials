using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Functions;
namespace Xbim.Ifc4.ActorResource {
public partial class IfcActorRole : IPersistValidatable {
public string WhereRule() {
string retVal = "";
if ((Role != IfcRoleEnum.USERDEFINED) || ((Role == IfcRoleEnum.USERDEFINED) && Function.EXISTS(this.UserDefinedRole))) 
{ // WR1
	retVal += "Where clause WR1 failed.";
}
return retVal;
}
}
}
