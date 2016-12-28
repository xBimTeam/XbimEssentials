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
	public partial class IfcTextStyleFontModel : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcTextStyleFontModel");

		/// <summary>
		/// Tests the express where clause MeasureOfFontSize
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MeasureOfFontSize() {
			var retVal = false;
			try {
				retVal = (TYPEOF(this.FontSize).Contains("IFC4.IFCLENGTHMEASURE")) && (this.FontSize.AsIfcLengthMeasure() > 0);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MeasureOfFontSize' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!MeasureOfFontSize())
				yield return new ValidationResult() { Item = this, IssueSource = "MeasureOfFontSize", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
