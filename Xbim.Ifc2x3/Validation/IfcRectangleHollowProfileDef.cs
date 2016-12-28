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
namespace Xbim.Ifc2x3.ProfileResource
{
	public partial class IfcRectangleHollowProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcRectangleHollowProfileDef");

		/// <summary>
		/// Tests the express where clause WR31
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR31() {
			var retVal = false;
			try {
				retVal = (WallThickness < (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (WallThickness < (this/* as IfcRectangleProfileDef*/.YDim / 2));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR31' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR32
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR32() {
			var retVal = false;
			try {
				retVal = !(EXISTS(OuterFilletRadius)) || ((OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR32' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR33
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR33() {
			var retVal = false;
			try {
				retVal = !(EXISTS(InnerFilletRadius)) || ((InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2 - WallThickness)) && (InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2 - WallThickness)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR33' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR31())
				yield return new ValidationResult() { Item = this, IssueSource = "WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR32())
				yield return new ValidationResult() { Item = this, IssueSource = "WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR33())
				yield return new ValidationResult() { Item = this, IssueSource = "WR33", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
