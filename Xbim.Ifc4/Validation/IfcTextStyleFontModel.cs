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
	public partial class IfcTextStyleFontModel : IExpressValidatable
	{
		public enum IfcTextStyleFontModelClause
		{
			MeasureOfFontSize,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTextStyleFontModelClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTextStyleFontModelClause.MeasureOfFontSize:
						retVal = (Functions.TYPEOF(this.FontSize).Contains("IFC4.IFCLENGTHMEASURE")) && (this.FontSize.AsIfcLengthMeasure() > 0);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PresentationAppearanceResource.IfcTextStyleFontModel>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTextStyleFontModel.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTextStyleFontModelClause.MeasureOfFontSize))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextStyleFontModel.MeasureOfFontSize", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
