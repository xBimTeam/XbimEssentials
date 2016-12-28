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
	public partial class IfcBSplineCurveWithKnots : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcBSplineCurveWithKnots");

		/// <summary>
		/// Tests the express where clause ConsistentBSpline
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ConsistentBSpline() {
			var retVal = false;
			try {
				retVal = IfcConstraintsParamBSpline(Degree, UpperIndexOnKnots, UpperIndexOnControlPoints, KnotMultiplicities, Knots);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ConsistentBSpline' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause CorrespondingKnotLists
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrespondingKnotLists() {
			var retVal = false;
			try {
				retVal = SIZEOF(KnotMultiplicities) == UpperIndexOnKnots;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrespondingKnotLists' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ConsistentBSpline())
				yield return new ValidationResult() { Item = this, IssueSource = "ConsistentBSpline", IssueType = ValidationFlags.EntityWhereClauses };
			if (!CorrespondingKnotLists())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrespondingKnotLists", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
