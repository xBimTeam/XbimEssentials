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
	public partial class IfcRectangularTrimmedSurface : IExpressValidatable
	{
		public enum IfcRectangularTrimmedSurfaceClause
		{
			U1AndU2Different,
			V1AndV2Different,
			UsenseCompatible,
			VsenseCompatible,
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
					case IfcRectangularTrimmedSurfaceClause.U1AndU2Different:
						retVal = U1 != U2;
						break;
					case IfcRectangularTrimmedSurfaceClause.V1AndV2Different:
						retVal = V1 != V2;
						break;
					case IfcRectangularTrimmedSurfaceClause.UsenseCompatible:
						retVal = ((Functions.TYPEOF(BasisSurface).Contains("IFC4.IFCELEMENTARYSURFACE")) && (!(Functions.TYPEOF(BasisSurface).Contains("IFC4.IFCPLANE")))) || (Functions.TYPEOF(BasisSurface).Contains("IFC4.IFCSURFACEOFREVOLUTION")) || (Usense == (U2 > U1));
						break;
					case IfcRectangularTrimmedSurfaceClause.VsenseCompatible:
						retVal = Vsense == (V2 > V1);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcRectangularTrimmedSurface>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRectangularTrimmedSurfaceClause.U1AndU2Different))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.U1AndU2Different", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangularTrimmedSurfaceClause.V1AndV2Different))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.V1AndV2Different", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangularTrimmedSurfaceClause.UsenseCompatible))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.UsenseCompatible", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRectangularTrimmedSurfaceClause.VsenseCompatible))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.VsenseCompatible", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
