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
		/// Tests the express where clause WR1
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR1() {
			var retVal = false;
			try {
				retVal = this/* as IfcPlacement*/.Location.Dim == 3;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR1' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR2
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR2() {
			var retVal = false;
			try {
				retVal = (!(EXISTS(Axis))) || (Axis.Dim == 3);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR2' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR3
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR3() {
			var retVal = false;
			try {
				retVal = (!(EXISTS(RefDirection))) || (RefDirection.Dim == 3);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR3' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR4
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR4() {
			var retVal = false;
			try {
				retVal = (!(EXISTS(Axis))) || (!(EXISTS(RefDirection))) || (IfcCrossProduct(Axis, RefDirection).Magnitude > 0);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR4' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR5
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR5() {
			var retVal = false;
			try {
				retVal = !((EXISTS(Axis)) ^ (EXISTS(RefDirection)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR5' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR1())
				yield return new ValidationResult() { Item = this, IssueSource = "WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR2())
				yield return new ValidationResult() { Item = this, IssueSource = "WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR3())
				yield return new ValidationResult() { Item = this, IssueSource = "WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR4())
				yield return new ValidationResult() { Item = this, IssueSource = "WR4", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR5())
				yield return new ValidationResult() { Item = this, IssueSource = "WR5", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
