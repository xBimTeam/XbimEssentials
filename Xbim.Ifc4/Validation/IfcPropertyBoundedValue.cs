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
namespace Xbim.Ifc4.PropertyResource
{
	public partial class IfcPropertyBoundedValue : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PropertyResource.IfcPropertyBoundedValue");

		/// <summary>
		/// Tests the express where clause SameUnitUpperLower
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SameUnitUpperLower() {
			var retVal = false;
			try {
				retVal = !(EXISTS(UpperBoundValue)) || !(EXISTS(LowerBoundValue)) || (TYPEOF(UpperBoundValue) == TYPEOF(LowerBoundValue));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SameUnitUpperLower' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause SameUnitUpperSet
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SameUnitUpperSet() {
			var retVal = false;
			try {
				retVal = !(EXISTS(UpperBoundValue)) || !(EXISTS(SetPointValue)) || (TYPEOF(UpperBoundValue) == TYPEOF(SetPointValue));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SameUnitUpperSet' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause SameUnitLowerSet
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SameUnitLowerSet() {
			var retVal = false;
			try {
				retVal = !(EXISTS(LowerBoundValue)) || !(EXISTS(SetPointValue)) || (TYPEOF(LowerBoundValue) == TYPEOF(SetPointValue));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SameUnitLowerSet' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!SameUnitUpperLower())
				yield return new ValidationResult() { Item = this, IssueSource = "SameUnitUpperLower", IssueType = ValidationFlags.EntityWhereClauses };
			if (!SameUnitUpperSet())
				yield return new ValidationResult() { Item = this, IssueSource = "SameUnitUpperSet", IssueType = ValidationFlags.EntityWhereClauses };
			if (!SameUnitLowerSet())
				yield return new ValidationResult() { Item = this, IssueSource = "SameUnitLowerSet", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
