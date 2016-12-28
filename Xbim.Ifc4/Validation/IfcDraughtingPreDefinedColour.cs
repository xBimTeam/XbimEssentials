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
	public partial class IfcDraughtingPreDefinedColour : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcDraughtingPreDefinedColour");

		/// <summary>
		/// Tests the express where clause PreDefinedColourNames
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool PreDefinedColourNames() {
			var retVal = false;
			try {
				retVal = NewArray("black", "red", "green", "blue", "yellow", "magenta", "cyan", "white", "by layer").Contains(this/* as IfcPreDefinedItem*/.Name);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'PreDefinedColourNames' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!PreDefinedColourNames())
				yield return new ValidationResult() { Item = this, IssueSource = "PreDefinedColourNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
