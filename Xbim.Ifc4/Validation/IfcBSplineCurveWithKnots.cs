using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcBSplineCurveWithKnots : IExpressValidatable
	{
		public enum IfcBSplineCurveWithKnotsClause
		{
			ConsistentBSpline,
			CorrespondingKnotLists,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcBSplineCurveWithKnotsClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBSplineCurveWithKnotsClause.ConsistentBSpline:
						retVal = Functions.IfcConstraintsParamBSpline(Degree, UpperIndexOnKnots, UpperIndexOnControlPoints, KnotMultiplicities, Knots);
						break;
					case IfcBSplineCurveWithKnotsClause.CorrespondingKnotLists:
						retVal = Functions.SIZEOF(KnotMultiplicities) == UpperIndexOnKnots;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcBSplineCurveWithKnots>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcBSplineCurveWithKnots.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcBSplineCurveWithKnotsClause.ConsistentBSpline))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineCurveWithKnots.ConsistentBSpline", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBSplineCurveWithKnotsClause.CorrespondingKnotLists))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineCurveWithKnots.CorrespondingKnotLists", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
