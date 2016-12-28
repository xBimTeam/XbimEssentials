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
	public partial class IfcCompositeCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcCompositeCurve");

		/// <summary>
		/// Tests the express where clause WR41
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR41() {
			var retVal = false;
			try {
				retVal = ((!ClosedCurve.Value) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 1)) || ((ClosedCurve.Value) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 0));
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
				retVal = SIZEOF(Segments.Where(Temp => Temp.Dim != Segments.ToArray()[0].Dim)) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR42' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR41())
				yield return new ValidationResult() { Item = this, IssueSource = "WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR42())
				yield return new ValidationResult() { Item = this, IssueSource = "WR42", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
