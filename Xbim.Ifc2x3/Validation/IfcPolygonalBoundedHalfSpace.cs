using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.GeometricModelResource
{
	public partial class IfcPolygonalBoundedHalfSpace : IExpressValidatable
	{
		public enum IfcPolygonalBoundedHalfSpaceClause
		{
			WR41,
			WR42,
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
					case IfcPolygonalBoundedHalfSpaceClause.WR41:
						retVal = PolygonalBoundary.Dim == 2;
						break;
					case IfcPolygonalBoundedHalfSpaceClause.WR42:
						retVal = Functions.SIZEOF(Functions.TYPEOF(PolygonalBoundary) * Functions.NewArray("IFC2X3.IFCPOLYLINE", "IFC2X3.IFCCOMPOSITECURVE")) == 1;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometricModelResource.IfcPolygonalBoundedHalfSpace>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPolygonalBoundedHalfSpace.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPolygonalBoundedHalfSpaceClause.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolygonalBoundedHalfSpace.WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPolygonalBoundedHalfSpaceClause.WR42))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolygonalBoundedHalfSpace.WR42", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
