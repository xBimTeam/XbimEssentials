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
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcRectangleHollowProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcRectangleHollowProfileDef");

		/// <summary>
		/// Tests the express where clause ValidWallThickness
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidWallThickness() {
			var retVal = false;
			try {
				retVal = (WallThickness < (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (WallThickness < (this/* as IfcRectangleProfileDef*/.YDim / 2));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidWallThickness' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ValidInnerRadius
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidInnerRadius() {
			var retVal = false;
			try {
				retVal = !(EXISTS(InnerFilletRadius)) || ((InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2 - WallThickness)) && (InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2 - WallThickness)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidInnerRadius' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ValidOuterRadius
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidOuterRadius() {
			var retVal = false;
			try {
				retVal = !(EXISTS(OuterFilletRadius)) || ((OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidOuterRadius' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidWallThickness())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidWallThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidInnerRadius())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidInnerRadius", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidOuterRadius())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidOuterRadius", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
