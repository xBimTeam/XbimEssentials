using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
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
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcProtectiveDeviceTrippingUnitTypeEnum.USERDEFINED) || ((PredefinedType == IfcProtectiveDeviceTrippingUnitTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcProtectiveDeviceTrippingUnitClause.CorrectTypeAssigned:
						retVal = (Functions.SIZEOF(IsTypedBy) == 0) || (Functions.TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCPROTECTIVEDEVICETRIPPINGUNITTYPE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ElectricalDomain.IfcProtectiveDeviceTrippingUnit>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcProtectiveDeviceTrippingUnit.{0}' for #{1}.", clause,EntityLabel), ex);
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
