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

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBSplineCurveWithKnots clause) {
			var retVal = false;
			if (clause == Where.IfcBSplineCurveWithKnots.ConsistentBSpline) {
				try {
					retVal = IfcConstraintsParamBSpline(Degree, UpperIndexOnKnots, UpperIndexOnControlPoints, KnotMultiplicities, Knots);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcBSplineCurveWithKnots");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcBSplineCurveWithKnots.ConsistentBSpline' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBSplineCurveWithKnots.CorrespondingKnotLists) {
				try {
					retVal = SIZEOF(KnotMultiplicities) == UpperIndexOnKnots;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcBSplineCurveWithKnots");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcBSplineCurveWithKnots.CorrespondingKnotLists' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcBSplineCurve)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcBSplineCurveWithKnots.ConsistentBSpline))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineCurveWithKnots.ConsistentBSpline", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBSplineCurveWithKnots.CorrespondingKnotLists))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineCurveWithKnots.CorrespondingKnotLists", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcBSplineCurveWithKnots : IfcBSplineCurve
	{
		public static readonly IfcBSplineCurveWithKnots ConsistentBSpline = new IfcBSplineCurveWithKnots();
		public static readonly IfcBSplineCurveWithKnots CorrespondingKnotLists = new IfcBSplineCurveWithKnots();
		protected IfcBSplineCurveWithKnots() {}
	}
}
