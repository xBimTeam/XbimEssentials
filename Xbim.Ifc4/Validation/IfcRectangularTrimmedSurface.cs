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
	public partial class IfcRectangularTrimmedSurface : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcRectangularTrimmedSurface");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRectangularTrimmedSurface clause) {
			var retVal = false;
			if (clause == Where.IfcRectangularTrimmedSurface.U1AndU2Different) {
				try {
					retVal = U1 != U2;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.U1AndU2Different' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangularTrimmedSurface.V1AndV2Different) {
				try {
					retVal = V1 != V2;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.V1AndV2Different' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangularTrimmedSurface.UsenseCompatible) {
				try {
					retVal = ((TYPEOF(BasisSurface).Contains("IFC4.IFCELEMENTARYSURFACE")) && (!(TYPEOF(BasisSurface).Contains("IFC4.IFCPLANE")))) || (TYPEOF(BasisSurface).Contains("IFC4.IFCSURFACEOFREVOLUTION")) || (Usense == (U2 > U1));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.UsenseCompatible' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangularTrimmedSurface.VsenseCompatible) {
				try {
					retVal = Vsense == (V2 > V1);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.VsenseCompatible' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRectangularTrimmedSurface.U1AndU2Different))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.U1AndU2Different", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangularTrimmedSurface.V1AndV2Different))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.V1AndV2Different", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangularTrimmedSurface.UsenseCompatible))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.UsenseCompatible", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangularTrimmedSurface.VsenseCompatible))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.VsenseCompatible", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRectangularTrimmedSurface
	{
		public static readonly IfcRectangularTrimmedSurface U1AndU2Different = new IfcRectangularTrimmedSurface();
		public static readonly IfcRectangularTrimmedSurface V1AndV2Different = new IfcRectangularTrimmedSurface();
		public static readonly IfcRectangularTrimmedSurface UsenseCompatible = new IfcRectangularTrimmedSurface();
		public static readonly IfcRectangularTrimmedSurface VsenseCompatible = new IfcRectangularTrimmedSurface();
		protected IfcRectangularTrimmedSurface() {}
	}
}
