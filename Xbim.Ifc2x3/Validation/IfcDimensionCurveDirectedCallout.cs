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
	public partial class IfcDimensionCurveDirectedCallout : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCurveDirectedCallout");

		/// <summary>
		/// Tests the express where clause WR41
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR41() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcDraughtingCallout*/.Contents.Where(Dc => (TYPEOF(Dc).Contains("IFC2X3.IFCDIMENSIONCURVE")))) == 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR41' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR42
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR42() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.Contents.Where(Dc => (TYPEOF(Dc).Contains("IFC2X3.IFCPROJECTIONCURVE")))) <= 2;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR42' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR41())
				yield return new ValidationResult() { Item = this, IssueSource = "WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR42())
				yield return new ValidationResult() { Item = this, IssueSource = "WR42", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
