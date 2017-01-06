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
	public partial class IfcRationalBezierCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcRationalBezierCurve");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRationalBezierCurve clause) {
			var retVal = false;
			if (clause == Where.IfcRationalBezierCurve.WR1) {
				try {
					retVal = SIZEOF(WeightsData) == SIZEOF(this/* as IfcBSplineCurve*/.ControlPointsList);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRationalBezierCurve.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRationalBezierCurve.WR2) {
				try {
					retVal = IfcCurveWeightsPositive(this);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRationalBezierCurve.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcBSplineCurve)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcRationalBezierCurve.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRationalBezierCurve.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRationalBezierCurve.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRationalBezierCurve.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcRationalBezierCurve : IfcBSplineCurve
	{
		public static readonly IfcRationalBezierCurve WR1 = new IfcRationalBezierCurve();
		public static readonly IfcRationalBezierCurve WR2 = new IfcRationalBezierCurve();
		protected IfcRationalBezierCurve() {}
	}
}
