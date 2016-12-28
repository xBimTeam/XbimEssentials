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
	public partial class IfcCompositeCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCompositeCurve");

		/// <summary>
		/// Tests the express where clause CurveContinuous
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CurveContinuous() {
			var retVal = false;
			try {
				retVal = ((!ClosedCurve.AsBool()) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 1)) || ((ClosedCurve.AsBool()) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 0));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CurveContinuous' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause SameDim
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SameDim() {
			var retVal = false;
			try {
				retVal = SIZEOF(Segments.Where(Temp => Temp.Dim != Segments.ToArray()[0].Dim)) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SameDim' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!CurveContinuous())
				yield return new ValidationResult() { Item = this, IssueSource = "CurveContinuous", IssueType = ValidationFlags.EntityWhereClauses };
			if (!SameDim())
				yield return new ValidationResult() { Item = this, IssueSource = "SameDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
