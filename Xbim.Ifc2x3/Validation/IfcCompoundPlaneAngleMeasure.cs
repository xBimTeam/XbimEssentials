using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.MeasureResource
{
	public partial struct IfcCompoundPlaneAngleMeasure 
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcCompoundPlaneAngleMeasure");

		/// <summary>
		/// Tests the express where clause WR1
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR1() {
			var retVal = false;
			try {
				retVal = ((-360 <= this.ToArray()[0]) && (this.ToArray()[0] < 360) );
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR1'.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR2
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR2() {
			var retVal = false;
			try {
				retVal = ((-60 <= this.ToArray()[1]) && (this.ToArray()[1] < 60) );
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR2'.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR3
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR3() {
			var retVal = false;
			try {
				retVal = ((-60 <= this.ToArray()[2]) && (this.ToArray()[2] < 60) );
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR3'.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR4
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR4() {
			var retVal = false;
			try {
				retVal = ((this.ToArray()[0] >= 0) && (this.ToArray()[1] >= 0) && (this.ToArray()[2] >= 0)) || ((this.ToArray()[0] <= 0) && (this.ToArray()[1] <= 0) && (this.ToArray()[2] <= 0));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR4'.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR1())
				yield return new ValidationResult() { Item = this, IssueSource = "WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR2())
				yield return new ValidationResult() { Item = this, IssueSource = "WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR3())
				yield return new ValidationResult() { Item = this, IssueSource = "WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR4())
				yield return new ValidationResult() { Item = this, IssueSource = "WR4", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
