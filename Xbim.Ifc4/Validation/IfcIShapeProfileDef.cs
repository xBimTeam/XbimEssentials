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
	public partial class IfcIShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcIShapeProfileDef");

		/// <summary>
		/// Tests the express where clause ValidFlangeThickness
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidFlangeThickness() {
			var retVal = false;
			try {
				retVal = (2 * FlangeThickness) < OverallDepth;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidFlangeThickness' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ValidWebThickness
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidWebThickness() {
			var retVal = false;
			try {
				retVal = WebThickness < OverallWidth;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidWebThickness' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ValidFilletRadius
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidFilletRadius() {
			var retVal = false;
			try {
				retVal = !(EXISTS(FilletRadius)) || ((FilletRadius <= (OverallWidth - WebThickness) / 2) && (FilletRadius <= (OverallDepth - (2 * FlangeThickness)) / 2));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidFilletRadius' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidFlangeThickness())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidWebThickness())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidWebThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidFilletRadius())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidFilletRadius", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
