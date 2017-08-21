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
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class IfcRectangularTrimmedSurface : IExpressValidatable
	{
		public enum IfcRectangularTrimmedSurfaceClause
		{
			WR1,
			WR2,
			WR3,
			WR4,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRectangularTrimmedSurfaceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRectangularTrimmedSurfaceClause.WR1:
						retVal = U1 != U2;
						break;
					case IfcRectangularTrimmedSurfaceClause.WR2:
						retVal = V1 != V2;
						break;
					case IfcRectangularTrimmedSurfaceClause.WR3:
						retVal = ((Functions.TYPEOF(BasisSurface).Contains("IFC2X3.IFCELEMENTARYSURFACE")) && (!(Functions.TYPEOF(BasisSurface).Contains("IFC2X3.IFCPLANE")))) || (Functions.TYPEOF(BasisSurface).Contains("IFC2X3.IFCSURFACEOFREVOLUTION")) || (Usense == (U2 > U1));
						break;
					case IfcRectangularTrimmedSurfaceClause.WR4:
						retVal = Vsense == (V2 > V1);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometryResource.IfcRectangularTrimmedSurface>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRectangularTrimmedSurfaceClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangularTrimmedSurfaceClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangularTrimmedSurfaceClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangularTrimmedSurfaceClause.WR4))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.WR4", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
