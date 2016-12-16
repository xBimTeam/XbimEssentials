using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Functions;
namespace Xbim.Ifc4.BuildingControlsDomain {
public partial class IfcActuator : IPersistValidatable {
public string WhereRule() {
string retVal = "";
if (!(EXISTS(PredefinedType)) || (PredefinedType != IfcActuatorTypeEnum.USERDEFINED) || ((PredefinedType == IfcActuatorTypeEnum.USERDEFINED) && EXISTS(this.ObjectType))) 
{ // CorrectPredefinedType
	retVal += "Where clause CorrectPredefinedType failed.";
}if ((SIZEOF(IsTypedBy) == 0) || (TYPEOF(this.IsTypedBy[1-1].RelatingType).Contains("IFC4.IFCACTUATORTYPE"))) 
{ // CorrectTypeAssigned
	retVal += "Where clause CorrectTypeAssigned failed.";
}
return retVal;
}
}
}
