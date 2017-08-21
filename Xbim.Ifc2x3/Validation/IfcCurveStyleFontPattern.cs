using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial class IfcCurveStyleFontPattern : IExpressValidatable
	{
		public enum IfcCurveStyleFontPatternClause
		{
			WR01,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCurveStyleFontPatternClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCurveStyleFontPatternClause.WR01:
						retVal = VisibleSegmentLength >= 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationAppearanceResource.IfcCurveStyleFontPattern>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCurveStyleFontPattern.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCurveStyleFontPatternClause.WR01))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCurveStyleFontPattern.WR01", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
