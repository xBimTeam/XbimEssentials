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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCurveStyleFontPattern clause) {
			var retVal = false;
			if (clause == Where.IfcCurveStyleFontPattern.VisibleLengthGreaterEqualZero) {
				try {
					retVal = VisibleSegmentLength >= 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCurveStyleFontPattern.VisibleLengthGreaterEqualZero' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCurveStyleFontPattern.VisibleLengthGreaterEqualZero))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCurveStyleFontPattern.VisibleLengthGreaterEqualZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCurveStyleFontPattern
	{
		public static readonly IfcCurveStyleFontPattern VisibleLengthGreaterEqualZero = new IfcCurveStyleFontPattern();
		protected IfcCurveStyleFontPattern() {}
	}
}
