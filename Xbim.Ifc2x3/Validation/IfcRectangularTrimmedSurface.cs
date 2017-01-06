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
	public partial class IfcRectangularTrimmedSurface : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcRectangularTrimmedSurface");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRectangularTrimmedSurface clause) {
			var retVal = false;
			if (clause == Where.IfcRectangularTrimmedSurface.WR1) {
				try {
					retVal = U1 != U2;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangularTrimmedSurface.WR2) {
				try {
					retVal = V1 != V2;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangularTrimmedSurface.WR3) {
				try {
					retVal = ((TYPEOF(BasisSurface).Contains("IFC2X3.IFCELEMENTARYSURFACE")) && (!(TYPEOF(BasisSurface).Contains("IFC2X3.IFCPLANE")))) || (TYPEOF(BasisSurface).Contains("IFC2X3.IFCSURFACEOFREVOLUTION")) || (Usense == (U2 > U1));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.WR3' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangularTrimmedSurface.WR4) {
				try {
					retVal = Vsense == (V2 > V1);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangularTrimmedSurface.WR4' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRectangularTrimmedSurface.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangularTrimmedSurface.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangularTrimmedSurface.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangularTrimmedSurface.WR4))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangularTrimmedSurface.WR4", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcRectangularTrimmedSurface
	{
		public static readonly IfcRectangularTrimmedSurface WR1 = new IfcRectangularTrimmedSurface();
		public static readonly IfcRectangularTrimmedSurface WR2 = new IfcRectangularTrimmedSurface();
		public static readonly IfcRectangularTrimmedSurface WR3 = new IfcRectangularTrimmedSurface();
		public static readonly IfcRectangularTrimmedSurface WR4 = new IfcRectangularTrimmedSurface();
		protected IfcRectangularTrimmedSurface() {}
	}
}
