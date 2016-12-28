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
	public partial class IfcCurveStyle : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcCurveStyle");

		/// <summary>
		/// Tests the express where clause MeasureOfWidth
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MeasureOfWidth() {
			var retVal = false;
			try {
				retVal = (!(EXISTS(CurveWidth))) || (TYPEOF(CurveWidth).Contains("IFC4.IFCPOSITIVELENGTHMEASURE")) || ((TYPEOF(CurveWidth).Contains("IFC4.IFCDESCRIPTIVEMEASURE")) && (CurveWidth.AsIfcDescriptiveMeasure() == "by layer"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MeasureOfWidth' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause IdentifiableCurveStyle
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool IdentifiableCurveStyle() {
			var retVal = false;
			try {
				retVal = EXISTS(CurveFont) || EXISTS(CurveWidth) || EXISTS(CurveColour);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'IdentifiableCurveStyle' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!MeasureOfWidth())
				yield return new ValidationResult() { Item = this, IssueSource = "MeasureOfWidth", IssueType = ValidationFlags.EntityWhereClauses };
			if (!IdentifiableCurveStyle())
				yield return new ValidationResult() { Item = this, IssueSource = "IdentifiableCurveStyle", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
