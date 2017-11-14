using System;
using log4net;
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
	public partial class IfcRationalBSplineCurveWithKnots : IExpressValidatable
	{
		public enum IfcRationalBSplineCurveWithKnotsClause
		{
			SameNumOfWeightsAndPoints,
			WeightsGreaterZero,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRationalBSplineCurveWithKnotsClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRationalBSplineCurveWithKnotsClause.SameNumOfWeightsAndPoints:
						retVal = Functions.SIZEOF(WeightsData) == Functions.SIZEOF(this/* as IfcBSplineCurve*/.ControlPointsList);
						break;
					case IfcRationalBSplineCurveWithKnotsClause.WeightsGreaterZero:
						retVal = Functions.IfcCurveWeightsPositive(this);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcRationalBSplineCurveWithKnots");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRationalBSplineCurveWithKnots.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRationalBSplineCurveWithKnotsClause.SameNumOfWeightsAndPoints))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRationalBSplineCurveWithKnots.SameNumOfWeightsAndPoints", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRationalBSplineCurveWithKnotsClause.WeightsGreaterZero))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRationalBSplineCurveWithKnots.WeightsGreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
