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
	public partial class IfcRationalBSplineSurfaceWithKnots : IExpressValidatable
	{
		public enum IfcRationalBSplineSurfaceWithKnotsClause
		{
			CorrespondingWeightsDataLists,
			WeightValuesGreaterZero,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRationalBSplineSurfaceWithKnotsClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRationalBSplineSurfaceWithKnotsClause.CorrespondingWeightsDataLists:
						retVal = (Functions.SIZEOF(WeightsData) == Functions.SIZEOF(this/* as IfcBSplineSurface*/.ControlPointsList)) && (Functions.SIZEOF(WeightsData.ItemAt(0)) == Functions.SIZEOF(this/* as IfcBSplineSurface*/.ControlPointsList.ItemAt(0)));
						break;
					case IfcRationalBSplineSurfaceWithKnotsClause.WeightValuesGreaterZero:
						retVal = Functions.IfcSurfaceWeightsPositive(this);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcRationalBSplineSurfaceWithKnots>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRationalBSplineSurfaceWithKnots.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRationalBSplineSurfaceWithKnotsClause.CorrespondingWeightsDataLists))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRationalBSplineSurfaceWithKnots.CorrespondingWeightsDataLists", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRationalBSplineSurfaceWithKnotsClause.WeightValuesGreaterZero))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRationalBSplineSurfaceWithKnots.WeightValuesGreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
