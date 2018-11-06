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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcPolygonalBoundedHalfSpace : IExpressValidatable
	{
		public enum IfcPolygonalBoundedHalfSpaceClause
		{
			BoundaryDim,
			BoundaryType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPolygonalBoundedHalfSpaceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPolygonalBoundedHalfSpaceClause.BoundaryDim:
						retVal = PolygonalBoundary.Dim == 2;
						break;
					case IfcPolygonalBoundedHalfSpaceClause.BoundaryType:
						retVal = Functions.SIZEOF(Functions.TYPEOF(PolygonalBoundary) * Functions.NewTypesArray("IFC4.IFCPOLYLINE", "IFC4.IFCCOMPOSITECURVE")) == 1;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcPolygonalBoundedHalfSpace>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPolygonalBoundedHalfSpace.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPolygonalBoundedHalfSpaceClause.BoundaryDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolygonalBoundedHalfSpace.BoundaryDim", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPolygonalBoundedHalfSpaceClause.BoundaryType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolygonalBoundedHalfSpace.BoundaryType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
