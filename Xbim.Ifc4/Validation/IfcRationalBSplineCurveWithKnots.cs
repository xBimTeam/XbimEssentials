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
	public partial class IfcRationalBSplineCurveWithKnots : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcRationalBSplineCurveWithKnots");

		/// <summary>
		/// Tests the express where clause SameNumOfWeightsAndPoints
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SameNumOfWeightsAndPoints() {
			var retVal = false;
			try {
				retVal = SIZEOF(WeightsData) == SIZEOF(this/* as IfcBSplineCurve*/.ControlPointsList);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SameNumOfWeightsAndPoints' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WeightsGreaterZero
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WeightsGreaterZero() {
			var retVal = false;
			try {
				retVal = IfcCurveWeightsPositive(this);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WeightsGreaterZero' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!SameNumOfWeightsAndPoints())
				yield return new ValidationResult() { Item = this, IssueSource = "SameNumOfWeightsAndPoints", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WeightsGreaterZero())
				yield return new ValidationResult() { Item = this, IssueSource = "WeightsGreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
