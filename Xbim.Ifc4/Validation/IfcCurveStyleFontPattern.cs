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
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial class IfcCurveStyleFontPattern : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcCurveStyleFontPattern");

		/// <summary>
		/// Tests the express where clause VisibleLengthGreaterEqualZero
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool VisibleLengthGreaterEqualZero() {
			var retVal = false;
			try {
				retVal = VisibleSegmentLength >= 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'VisibleLengthGreaterEqualZero' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!VisibleLengthGreaterEqualZero())
				yield return new ValidationResult() { Item = this, IssueSource = "VisibleLengthGreaterEqualZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
