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
	public partial class IfcRationalBSplineSurfaceWithKnots : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcRationalBSplineSurfaceWithKnots");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRationalBSplineSurfaceWithKnots clause) {
			var retVal = false;
			if (clause == Where.IfcRationalBSplineSurfaceWithKnots.CorrespondingWeightsDataLists) {
				try {
					retVal = (SIZEOF(WeightsData) == SIZEOF(this/* as IfcBSplineSurface*/.ControlPointsList)) && (SIZEOF(WeightsData.ToArray()[0]) == SIZEOF(this/* as IfcBSplineSurface*/.ControlPointsList.ToArray()[0]));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRationalBSplineSurfaceWithKnots.CorrespondingWeightsDataLists' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRationalBSplineSurfaceWithKnots.WeightValuesGreaterZero) {
				try {
					retVal = IfcSurfaceWeightsPositive(this);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRationalBSplineSurfaceWithKnots.WeightValuesGreaterZero' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcBSplineSurfaceWithKnots)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcRationalBSplineSurfaceWithKnots.CorrespondingWeightsDataLists))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRationalBSplineSurfaceWithKnots.CorrespondingWeightsDataLists", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRationalBSplineSurfaceWithKnots.WeightValuesGreaterZero))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRationalBSplineSurfaceWithKnots.WeightValuesGreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRationalBSplineSurfaceWithKnots : IfcBSplineSurfaceWithKnots
	{
		public static readonly IfcRationalBSplineSurfaceWithKnots CorrespondingWeightsDataLists = new IfcRationalBSplineSurfaceWithKnots();
		public static readonly IfcRationalBSplineSurfaceWithKnots WeightValuesGreaterZero = new IfcRationalBSplineSurfaceWithKnots();
		protected IfcRationalBSplineSurfaceWithKnots() {}
	}
}
