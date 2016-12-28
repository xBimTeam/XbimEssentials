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
		/// Tests the express where clause WR21
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR21() {
			var retVal = false;
			try {
				retVal = (!(EXISTS(DefiningValues)) && !(EXISTS(DefinedValues))) || (SIZEOF(DefiningValues) == SIZEOF(DefinedValues));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR21' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR22
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR22() {
			var retVal = false;
			try {
				retVal = !(EXISTS(DefiningValues)) || (SIZEOF(this.DefiningValues.Where(temp => TYPEOF(temp) != TYPEOF(this.DefiningValues.ToArray()[0]))) == 0);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR22' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR23
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR23() {
			var retVal = false;
			try {
				retVal = !(EXISTS(DefinedValues)) || (SIZEOF(this.DefinedValues.Where(temp => TYPEOF(temp) != TYPEOF(this.DefinedValues.ToArray()[0]))) == 0);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR23' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR21())
				yield return new ValidationResult() { Item = this, IssueSource = "WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR22())
				yield return new ValidationResult() { Item = this, IssueSource = "WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR23())
				yield return new ValidationResult() { Item = this, IssueSource = "WR23", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
