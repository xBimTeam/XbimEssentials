using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial class IfcCurveStyle : IExpressValidatable
	{
		public enum IfcCurveStyleClause
		{
			MeasureOfWidth,
			IdentifiableCurveStyle,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCurveStyleClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCurveStyleClause.MeasureOfWidth:
						retVal = (!(Functions.EXISTS(CurveWidth))) || (Functions.TYPEOF(CurveWidth).Contains("IFC4.IFCPOSITIVELENGTHMEASURE")) || ((Functions.TYPEOF(CurveWidth).Contains("IFC4.IFCDESCRIPTIVEMEASURE")) && (CurveWidth.AsIfcDescriptiveMeasure() == "by layer"));
						break;
					case IfcCurveStyleClause.IdentifiableCurveStyle:
						retVal = Functions.EXISTS(CurveFont) || Functions.EXISTS(CurveWidth) || Functions.EXISTS(CurveColour);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PresentationAppearanceResource.IfcCurveStyle>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCurveStyle.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCurveStyleClause.MeasureOfWidth))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCurveStyle.MeasureOfWidth", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCurveStyleClause.IdentifiableCurveStyle))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCurveStyle.IdentifiableCurveStyle", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
