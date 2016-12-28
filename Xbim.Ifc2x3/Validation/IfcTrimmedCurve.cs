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
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class IfcTrimmedCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcTrimmedCurve");

		/// <summary>
		/// Tests the express where clause WR41
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR41() {
			var retVal = false;
			try {
				retVal = (HIINDEX(Trim1) == 1) || (TYPEOF(Trim1.ToArray()[0]) != TYPEOF(Trim1.ToArray()[1]));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR41' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR42
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR42() {
			var retVal = false;
			try {
				retVal = (HIINDEX(Trim2) == 1) || (TYPEOF(Trim2.ToArray()[0]) != TYPEOF(Trim2.ToArray()[1]));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR42' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR43
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR43() {
			var retVal = false;
			try {
				retVal = !(TYPEOF(BasisCurve).Contains("IFC2X3.IFCBOUNDEDCURVE"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR43' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR41())
				yield return new ValidationResult() { Item = this, IssueSource = "WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR42())
				yield return new ValidationResult() { Item = this, IssueSource = "WR42", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR43())
				yield return new ValidationResult() { Item = this, IssueSource = "WR43", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
