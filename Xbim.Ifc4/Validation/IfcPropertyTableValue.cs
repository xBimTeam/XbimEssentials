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
	public partial class IfcPropertyTableValue : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PropertyResource.IfcPropertyTableValue");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPropertyTableValue clause) {
			var retVal = false;
			if (clause == Where.IfcPropertyTableValue.WR21) {
				try {
					retVal = (!(EXISTS(DefiningValues)) && !(EXISTS(DefinedValues))) || (SIZEOF(DefiningValues) == SIZEOF(DefinedValues));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPropertyTableValue.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertyTableValue.WR22) {
				try {
					retVal = !(EXISTS(DefiningValues)) || (SIZEOF(this.DefiningValues.Where(temp => TYPEOF(temp) != TYPEOF(this.DefiningValues.ToArray()[0]))) == 0);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPropertyTableValue.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertyTableValue.WR23) {
				try {
					retVal = !(EXISTS(DefinedValues)) || (SIZEOF(this.DefinedValues.Where(temp => TYPEOF(temp) != TYPEOF(this.DefinedValues.ToArray()[0]))) == 0);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPropertyTableValue.WR23' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertyTableValue.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertyTableValue.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertyTableValue.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR23", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPropertyTableValue
	{
		public static readonly IfcPropertyTableValue WR21 = new IfcPropertyTableValue();
		public static readonly IfcPropertyTableValue WR22 = new IfcPropertyTableValue();
		public static readonly IfcPropertyTableValue WR23 = new IfcPropertyTableValue();
		protected IfcPropertyTableValue() {}
	}
}
