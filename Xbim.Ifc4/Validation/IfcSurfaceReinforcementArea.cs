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
namespace Xbim.Ifc4.StructuralLoadResource
{
	public partial class IfcSurfaceReinforcementArea : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralLoadResource.IfcSurfaceReinforcementArea");

		/// <summary>
		/// Tests the express where clause SurfaceAndOrShearAreaSpecified
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SurfaceAndOrShearAreaSpecified() {
			var retVal = false;
			try {
				retVal = EXISTS(SurfaceReinforcement1) || EXISTS(SurfaceReinforcement2) || EXISTS(ShearReinforcement);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SurfaceAndOrShearAreaSpecified' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NonnegativeArea1
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NonnegativeArea1() {
			var retVal = false;
			try {
				retVal = (!EXISTS(SurfaceReinforcement1)) || ((SurfaceReinforcement1.ToArray()[0] >= 0) && (SurfaceReinforcement1.ToArray()[1] >= 0) && ((SIZEOF(SurfaceReinforcement1) == 1) || (SurfaceReinforcement1.ToArray()[0] >= 0)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NonnegativeArea1' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NonnegativeArea2
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NonnegativeArea2() {
			var retVal = false;
			try {
				retVal = (!EXISTS(SurfaceReinforcement2)) || ((SurfaceReinforcement2.ToArray()[0] >= 0) && (SurfaceReinforcement2.ToArray()[1] >= 0) && ((SIZEOF(SurfaceReinforcement2) == 1) || (SurfaceReinforcement2.ToArray()[0] >= 0)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NonnegativeArea2' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NonnegativeArea3
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NonnegativeArea3() {
			var retVal = false;
			try {
				retVal = (!EXISTS(ShearReinforcement)) || (ShearReinforcement >= 0);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NonnegativeArea3' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!SurfaceAndOrShearAreaSpecified())
				yield return new ValidationResult() { Item = this, IssueSource = "SurfaceAndOrShearAreaSpecified", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NonnegativeArea1())
				yield return new ValidationResult() { Item = this, IssueSource = "NonnegativeArea1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NonnegativeArea2())
				yield return new ValidationResult() { Item = this, IssueSource = "NonnegativeArea2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NonnegativeArea3())
				yield return new ValidationResult() { Item = this, IssueSource = "NonnegativeArea3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
