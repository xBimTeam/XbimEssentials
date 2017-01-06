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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCurveStyle clause) {
			var retVal = false;
			if (clause == Where.IfcCurveStyle.MeasureOfWidth) {
				try {
					retVal = (!(EXISTS(CurveWidth))) || (TYPEOF(CurveWidth).Contains("IFC4.IFCPOSITIVELENGTHMEASURE")) || ((TYPEOF(CurveWidth).Contains("IFC4.IFCDESCRIPTIVEMEASURE")) && (CurveWidth.AsIfcDescriptiveMeasure() == "by layer"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCurveStyle.MeasureOfWidth' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCurveStyle.IdentifiableCurveStyle) {
				try {
					retVal = EXISTS(CurveFont) || EXISTS(CurveWidth) || EXISTS(CurveColour);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCurveStyle.IdentifiableCurveStyle' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCurveStyle.MeasureOfWidth))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCurveStyle.MeasureOfWidth", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCurveStyle.IdentifiableCurveStyle))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCurveStyle.IdentifiableCurveStyle", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCurveStyle
	{
		public static readonly IfcCurveStyle MeasureOfWidth = new IfcCurveStyle();
		public static readonly IfcCurveStyle IdentifiableCurveStyle = new IfcCurveStyle();
		protected IfcCurveStyle() {}
	}
}
