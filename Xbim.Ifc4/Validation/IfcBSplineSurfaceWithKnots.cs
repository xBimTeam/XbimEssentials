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
	public partial class IfcBSplineSurfaceWithKnots : IExpressValidatable
	{
		public enum IfcBSplineSurfaceWithKnotsClause
		{
			UDirectionConstraints,
			VDirectionConstraints,
			CorrespondingULists,
			CorrespondingVLists,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcBSplineSurfaceWithKnotsClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBSplineSurfaceWithKnotsClause.UDirectionConstraints:
						retVal = Functions.IfcConstraintsParamBSpline(this/* as IfcBSplineSurface*/.UDegree, KnotUUpper, this/* as IfcBSplineSurface*/.UUpper, UMultiplicities, UKnots);
						break;
					case IfcBSplineSurfaceWithKnotsClause.VDirectionConstraints:
						retVal = Functions.IfcConstraintsParamBSpline(this/* as IfcBSplineSurface*/.VDegree, KnotVUpper, this/* as IfcBSplineSurface*/.VUpper, VMultiplicities, VKnots);
						break;
					case IfcBSplineSurfaceWithKnotsClause.CorrespondingULists:
						retVal = Functions.SIZEOF(UMultiplicities) == KnotUUpper;
						break;
					case IfcBSplineSurfaceWithKnotsClause.CorrespondingVLists:
						retVal = Functions.SIZEOF(VMultiplicities) == KnotVUpper;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcBSplineSurfaceWithKnots>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcBSplineSurfaceWithKnots.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcBSplineSurfaceWithKnotsClause.UDirectionConstraints))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineSurfaceWithKnots.UDirectionConstraints", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBSplineSurfaceWithKnotsClause.VDirectionConstraints))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineSurfaceWithKnots.VDirectionConstraints", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBSplineSurfaceWithKnotsClause.CorrespondingULists))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineSurfaceWithKnots.CorrespondingULists", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBSplineSurfaceWithKnotsClause.CorrespondingVLists))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineSurfaceWithKnots.CorrespondingVLists", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
