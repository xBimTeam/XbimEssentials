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
	public partial class IfcCShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcCShapeProfileDef");

		/// <summary>
		/// Tests the express where clause ValidGirth
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidGirth() {
			var retVal = false;
			try {
				retVal = Girth < (Depth / 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidGirth' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ValidInternalFilletRadius
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidInternalFilletRadius() {
			var retVal = false;
			try {
				retVal = !(EXISTS(InternalFilletRadius)) || ((InternalFilletRadius <= Width / 2 - WallThickness) && (InternalFilletRadius <= Depth / 2 - WallThickness));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidInternalFilletRadius' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ValidWallThickness
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidWallThickness() {
			var retVal = false;
			try {
				retVal = (WallThickness < Width / 2) && (WallThickness < Depth / 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidWallThickness' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidGirth())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidGirth", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidInternalFilletRadius())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidInternalFilletRadius", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidWallThickness())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidWallThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
