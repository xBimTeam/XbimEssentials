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
	public partial class IfcAxis2Placement3D : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcAxis2Placement3D");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAxis2Placement3D clause) {
			var retVal = false;
			if (clause == Where.IfcAxis2Placement3D.WR1) {
				try {
					retVal = this/* as IfcPlacement*/.Location.Dim == 3;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAxis2Placement3D.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement3D.WR2) {
				try {
					retVal = (!(EXISTS(Axis))) || (Axis.Dim == 3);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAxis2Placement3D.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement3D.WR3) {
				try {
					retVal = (!(EXISTS(RefDirection))) || (RefDirection.Dim == 3);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAxis2Placement3D.WR3' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement3D.WR4) {
				try {
					retVal = (!(EXISTS(Axis))) || (!(EXISTS(RefDirection))) || (IfcCrossProduct(Axis, RefDirection).Magnitude > 0);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAxis2Placement3D.WR4' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAxis2Placement3D.WR5) {
				try {
					retVal = !((EXISTS(Axis)) ^ (EXISTS(RefDirection)));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAxis2Placement3D.WR5' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcAxis2Placement3D.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement3D.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement3D.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement3D.WR4))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR4", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAxis2Placement3D.WR5))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR5", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcAxis2Placement3D
	{
		public static readonly IfcAxis2Placement3D WR1 = new IfcAxis2Placement3D();
		public static readonly IfcAxis2Placement3D WR2 = new IfcAxis2Placement3D();
		public static readonly IfcAxis2Placement3D WR3 = new IfcAxis2Placement3D();
		public static readonly IfcAxis2Placement3D WR4 = new IfcAxis2Placement3D();
		public static readonly IfcAxis2Placement3D WR5 = new IfcAxis2Placement3D();
		protected IfcAxis2Placement3D() {}
	}
}
