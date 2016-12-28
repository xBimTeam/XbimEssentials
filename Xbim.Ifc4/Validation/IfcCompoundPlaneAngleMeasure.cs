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
namespace Xbim.Ifc4.MeasureResource
{
	public partial struct IfcCompoundPlaneAngleMeasure 
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.MeasureResource.IfcCompoundPlaneAngleMeasure");

		/// <summary>
		/// Tests the express where clause MinutesInRange
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MinutesInRange() {
			var retVal = false;
			try {
				retVal = ABS(this.ToArray()[1]) < 60;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MinutesInRange'.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause SecondsInRange
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SecondsInRange() {
			var retVal = false;
			try {
				retVal = ABS(this.ToArray()[2]) < 60;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SecondsInRange'.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause MicrosecondsInRange
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MicrosecondsInRange() {
			var retVal = false;
			try {
				retVal = (SIZEOF(this) == 3) || (ABS(this.ToArray()[3]) < 1000000);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MicrosecondsInRange'.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ConsistentSign
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ConsistentSign() {
			var retVal = false;
			try {
				retVal = ((this.ToArray()[0] >= 0) && (this.ToArray()[1] >= 0) && (this.ToArray()[2] >= 0) && ((SIZEOF(this) == 3) || (this.ToArray()[3] >= 0))) || ((this.ToArray()[0] <= 0) && (this.ToArray()[1] <= 0) && (this.ToArray()[2] <= 0) && ((SIZEOF(this) == 3) || (this.ToArray()[3] <= 0)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ConsistentSign'.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!MinutesInRange())
				yield return new ValidationResult() { Item = this, IssueSource = "MinutesInRange", IssueType = ValidationFlags.EntityWhereClauses };
			if (!SecondsInRange())
				yield return new ValidationResult() { Item = this, IssueSource = "SecondsInRange", IssueType = ValidationFlags.EntityWhereClauses };
			if (!MicrosecondsInRange())
				yield return new ValidationResult() { Item = this, IssueSource = "MicrosecondsInRange", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ConsistentSign())
				yield return new ValidationResult() { Item = this, IssueSource = "ConsistentSign", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
