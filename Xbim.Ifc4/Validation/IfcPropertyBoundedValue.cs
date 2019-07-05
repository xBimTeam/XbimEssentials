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
namespace Xbim.Ifc4.PropertyResource
{
	public partial class IfcPropertyBoundedValue : IExpressValidatable
	{
		public enum IfcPropertyBoundedValueClause
		{
			SameUnitUpperLower,
			SameUnitUpperSet,
			SameUnitLowerSet,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPropertyBoundedValueClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPropertyBoundedValueClause.SameUnitUpperLower:
						retVal = !(Functions.EXISTS(UpperBoundValue)) || !(Functions.EXISTS(LowerBoundValue)) || (Functions.TYPEOF(UpperBoundValue) == Functions.TYPEOF(LowerBoundValue));
						break;
					case IfcPropertyBoundedValueClause.SameUnitUpperSet:
						retVal = !(Functions.EXISTS(UpperBoundValue)) || !(Functions.EXISTS(SetPointValue)) || (Functions.TYPEOF(UpperBoundValue) == Functions.TYPEOF(SetPointValue));
						break;
					case IfcPropertyBoundedValueClause.SameUnitLowerSet:
						retVal = !(Functions.EXISTS(LowerBoundValue)) || !(Functions.EXISTS(SetPointValue)) || (Functions.TYPEOF(LowerBoundValue) == Functions.TYPEOF(SetPointValue));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PropertyResource.IfcPropertyBoundedValue>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPropertyBoundedValue.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPropertyBoundedValueClause.SameUnitUpperLower))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyBoundedValue.SameUnitUpperLower", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPropertyBoundedValueClause.SameUnitUpperSet))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyBoundedValue.SameUnitUpperSet", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPropertyBoundedValueClause.SameUnitLowerSet))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyBoundedValue.SameUnitLowerSet", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
