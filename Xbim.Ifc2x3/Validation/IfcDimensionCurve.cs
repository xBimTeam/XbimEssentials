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
	public partial class IfcDimensionCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCurve");

		/// <summary>
		/// Tests the express where clause WR51
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR51() {
			var retVal = false;
			try {
				retVal = SIZEOF(USEDIN(this, "IFC2X3.IFCDRAUGHTINGCALLOUT.CONTENTS")) >= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR51' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR52
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR52() {
			var retVal = false;
			try {
				retVal = (SIZEOF(USEDIN(this, "IFC2X3." + "IFCTERMINATORSYMBOL.ANNOTATEDCURVE").Where(Dct1 => (Dct1.AsIfcDimensionCurveTerminator().Role == IfcDimensionExtentUsage.ORIGIN))) <= 1) && (SIZEOF(USEDIN(this, "IFC2X3." + "IFCTERMINATORSYMBOL.ANNOTATEDCURVE").Where(Dct2 => (Dct2.AsIfcDimensionCurveTerminator().Role == IfcDimensionExtentUsage.TARGET))) <= 1);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR52' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR53
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR53() {
			var retVal = false;
			try {
				retVal = SIZEOF(AnnotatedBySymbols.Where(Dct => !(TYPEOF(Dct).Contains("IFC2X3.IFCDIMENSIONCURVETERMINATOR")))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR53' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!WR51())
				yield return new ValidationResult() { Item = this, IssueSource = "WR51", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR52())
				yield return new ValidationResult() { Item = this, IssueSource = "WR52", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR53())
				yield return new ValidationResult() { Item = this, IssueSource = "WR53", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
