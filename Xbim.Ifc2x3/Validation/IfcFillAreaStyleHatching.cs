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
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial class IfcFillAreaStyleHatching : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationAppearanceResource.IfcFillAreaStyleHatching");

		/// <summary>
		/// Tests the express where clause WR21
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR21() {
			var retVal = false;
			try {
				retVal = !(TYPEOF(StartOfNextHatchLine).Contains("IFC2X3.IFCTWODIRECTIONREPEATFACTOR"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR21' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR22
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR22() {
			var retVal = false;
			try {
				retVal = !(EXISTS(PatternStart)) || (PatternStart.Dim == 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR22' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR23
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR23() {
			var retVal = false;
			try {
				retVal = !(EXISTS(PointOfReferenceHatchLine)) || (PointOfReferenceHatchLine.Dim == 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR23' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR21())
				yield return new ValidationResult() { Item = this, IssueSource = "WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR22())
				yield return new ValidationResult() { Item = this, IssueSource = "WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR23())
				yield return new ValidationResult() { Item = this, IssueSource = "WR23", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
