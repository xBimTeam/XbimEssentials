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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcRectangularTrimmedSurface : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcRectangularTrimmedSurface");

		/// <summary>
		/// Tests the express where clause U1AndU2Different
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool U1AndU2Different() {
			var retVal = false;
			try {
				retVal = U1 != U2;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'U1AndU2Different' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause V1AndV2Different
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool V1AndV2Different() {
			var retVal = false;
			try {
				retVal = V1 != V2;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'V1AndV2Different' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause UsenseCompatible
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool UsenseCompatible() {
			var retVal = false;
			try {
				retVal = ((TYPEOF(BasisSurface).Contains("IFC4.IFCELEMENTARYSURFACE")) && (!(TYPEOF(BasisSurface).Contains("IFC4.IFCPLANE")))) || (TYPEOF(BasisSurface).Contains("IFC4.IFCSURFACEOFREVOLUTION")) || (Usense == (U2 > U1));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'UsenseCompatible' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause VsenseCompatible
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool VsenseCompatible() {
			var retVal = false;
			try {
				retVal = Vsense == (V2 > V1);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'VsenseCompatible' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!U1AndU2Different())
				yield return new ValidationResult() { Item = this, IssueSource = "U1AndU2Different", IssueType = ValidationFlags.EntityWhereClauses };
			if (!V1AndV2Different())
				yield return new ValidationResult() { Item = this, IssueSource = "V1AndV2Different", IssueType = ValidationFlags.EntityWhereClauses };
			if (!UsenseCompatible())
				yield return new ValidationResult() { Item = this, IssueSource = "UsenseCompatible", IssueType = ValidationFlags.EntityWhereClauses };
			if (!VsenseCompatible())
				yield return new ValidationResult() { Item = this, IssueSource = "VsenseCompatible", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
