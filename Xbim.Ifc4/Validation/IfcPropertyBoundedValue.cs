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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPropertyBoundedValue clause) {
			var retVal = false;
			if (clause == Where.IfcPropertyBoundedValue.SameUnitUpperLower) {
				try {
					retVal = !(EXISTS(UpperBoundValue)) || !(EXISTS(LowerBoundValue)) || (TYPEOF(UpperBoundValue) == TYPEOF(LowerBoundValue));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPropertyBoundedValue.SameUnitUpperLower' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertyBoundedValue.SameUnitUpperSet) {
				try {
					retVal = !(EXISTS(UpperBoundValue)) || !(EXISTS(SetPointValue)) || (TYPEOF(UpperBoundValue) == TYPEOF(SetPointValue));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPropertyBoundedValue.SameUnitUpperSet' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertyBoundedValue.SameUnitLowerSet) {
				try {
					retVal = !(EXISTS(LowerBoundValue)) || !(EXISTS(SetPointValue)) || (TYPEOF(LowerBoundValue) == TYPEOF(SetPointValue));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPropertyBoundedValue.SameUnitLowerSet' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertyBoundedValue.SameUnitUpperLower))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyBoundedValue.SameUnitUpperLower", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertyBoundedValue.SameUnitUpperSet))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyBoundedValue.SameUnitUpperSet", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertyBoundedValue.SameUnitLowerSet))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyBoundedValue.SameUnitLowerSet", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPropertyBoundedValue
	{
		public static readonly IfcPropertyBoundedValue SameUnitUpperLower = new IfcPropertyBoundedValue();
		public static readonly IfcPropertyBoundedValue SameUnitUpperSet = new IfcPropertyBoundedValue();
		public static readonly IfcPropertyBoundedValue SameUnitLowerSet = new IfcPropertyBoundedValue();
		protected IfcPropertyBoundedValue() {}
	}
}
