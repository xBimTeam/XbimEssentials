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
		public enum IfcProtectiveDeviceTrippingUnitClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcProtectiveDeviceTrippingUnitClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcProtectiveDeviceTrippingUnitClause.CorrectPredefinedType:
						retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcProtectiveDeviceTrippingUnitTypeEnum.USERDEFINED) || ((PredefinedType == IfcProtectiveDeviceTrippingUnitTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcProtectiveDeviceTrippingUnitClause.CorrectTypeAssigned:
						retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCPROTECTIVEDEVICETRIPPINGUNITTYPE"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcProtectiveDeviceTrippingUnit");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProtectiveDeviceTrippingUnit.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcProtectiveDeviceTrippingUnitClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProtectiveDeviceTrippingUnit.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProtectiveDeviceTrippingUnitClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProtectiveDeviceTrippingUnit.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
