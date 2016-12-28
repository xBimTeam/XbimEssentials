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
namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
	public partial class IfcDimensionPair : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionPair");

		/// <summary>
		/// Tests the express where clause WR11
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR11() {
			var retVal = false;
			try {
				retVal = NewArray("chained", "parallel").Contains(this.Name);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR11' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR12
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR12() {
			var retVal = false;
			try {
				retVal = SIZEOF(TYPEOF(this.RelatingDraughtingCallout) * NewArray("IFC2X3.IFCANGULARDIMENSION", "IFC2X3.IFCDIAMETERDIMENSION", "IFC2X3.IFCLINEARDIMENSION", "IFC2X3.IFCRADIUSDIMENSION")) == 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR12' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR13
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR13() {
			var retVal = false;
			try {
				retVal = SIZEOF(TYPEOF(this.RelatedDraughtingCallout) * NewArray("IFC2X3.IFCANGULARDIMENSION", "IFC2X3.IFCDIAMETERDIMENSION", "IFC2X3.IFCLINEARDIMENSION", "IFC2X3.IFCRADIUSDIMENSION")) == 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR13' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR11())
				yield return new ValidationResult() { Item = this, IssueSource = "WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR12())
				yield return new ValidationResult() { Item = this, IssueSource = "WR12", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR13())
				yield return new ValidationResult() { Item = this, IssueSource = "WR13", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
