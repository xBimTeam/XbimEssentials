using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ElectricalDomain
{
	public partial class IfcProtectiveDeviceTrippingUnit : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcProtectiveDeviceTrippingUnit clause) {
			var retVal = false;
			if (clause == Where.IfcProtectiveDeviceTrippingUnit.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcProtectiveDeviceTrippingUnitTypeEnum.USERDEFINED) || ((PredefinedType == IfcProtectiveDeviceTrippingUnitTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcProtectiveDeviceTrippingUnit");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProtectiveDeviceTrippingUnit.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcProtectiveDeviceTrippingUnit.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCPROTECTIVEDEVICETRIPPINGUNITTYPE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcProtectiveDeviceTrippingUnit");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProtectiveDeviceTrippingUnit.CorrectTypeAssigned' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcProtectiveDeviceTrippingUnit.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProtectiveDeviceTrippingUnit.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcProtectiveDeviceTrippingUnit.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProtectiveDeviceTrippingUnit.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcProtectiveDeviceTrippingUnit : IfcProduct
	{
		public static readonly IfcProtectiveDeviceTrippingUnit CorrectPredefinedType = new IfcProtectiveDeviceTrippingUnit();
		public static readonly IfcProtectiveDeviceTrippingUnit CorrectTypeAssigned = new IfcProtectiveDeviceTrippingUnit();
		protected IfcProtectiveDeviceTrippingUnit() {}
	}
}
