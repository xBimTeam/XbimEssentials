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
	public partial class IfcProtectiveDevice : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcProtectiveDevice");

		/// <summary>
		/// Tests the express where clause CorrectPredefinedType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectPredefinedType() {
			var retVal = false;
			try {
				retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcProtectiveDeviceTypeEnum.USERDEFINED) || ((PredefinedType == IfcProtectiveDeviceTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectPredefinedType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause CorrectTypeAssigned
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectTypeAssigned() {
			var retVal = false;
			try {
				retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCPROTECTIVEDEVICETYPE"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectTypeAssigned' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!CorrectPredefinedType())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!CorrectTypeAssigned())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
