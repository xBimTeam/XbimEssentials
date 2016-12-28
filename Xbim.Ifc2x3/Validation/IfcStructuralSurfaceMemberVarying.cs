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
namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
	public partial class IfcStructuralSurfaceMemberVarying : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.StructuralAnalysisDomain.IfcStructuralSurfaceMemberVarying");

		/// <summary>
		/// Tests the express where clause WR61
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR61() {
			var retVal = false;
			try {
				retVal = EXISTS(this/* as IfcStructuralSurfaceMember*/.Thickness);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR61' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR62
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR62() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.VaryingThicknessLocation.ShapeRepresentations.Where(temp => !(SIZEOF(temp/* as IfcRepresentation*/.Items) == 1))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR62' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR63
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR63() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.VaryingThicknessLocation.ShapeRepresentations.Where(temp => !((TYPEOF(temp/* as IfcRepresentation*/.Items.ToArray()[0]).Contains("IFC2X3.IFCCARTESIANPOINT")) || (TYPEOF(temp/* as IfcRepresentation*/.Items.ToArray()[0]).Contains("IFC2X3.IFCPOINTONSURFACE"))))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR63' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!WR61())
				yield return new ValidationResult() { Item = this, IssueSource = "WR61", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR62())
				yield return new ValidationResult() { Item = this, IssueSource = "WR62", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR63())
				yield return new ValidationResult() { Item = this, IssueSource = "WR63", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
