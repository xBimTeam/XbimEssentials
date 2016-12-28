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
	public partial class IfcDraughtingPreDefinedCurveFont : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcDraughtingPreDefinedCurveFont");

		/// <summary>
		/// Tests the express where clause PreDefinedCurveFontNames
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool PreDefinedCurveFontNames() {
			var retVal = false;
			try {
				retVal = NewArray("continuous", "chain", "chain double dash", "dashed", "dotted", "by layer").Contains(this/* as IfcPredefinedItem*/.Name);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'PreDefinedCurveFontNames' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!PreDefinedCurveFontNames())
				yield return new ValidationResult() { Item = this, IssueSource = "PreDefinedCurveFontNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
