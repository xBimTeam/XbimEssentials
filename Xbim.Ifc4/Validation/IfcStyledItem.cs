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
	public partial class IfcStyledItem : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcStyledItem");

		/// <summary>
		/// Tests the express where clause ApplicableItem
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ApplicableItem() {
			var retVal = false;
			try {
				retVal = !(TYPEOF(Item).Contains("IFC4.IFCSTYLEDITEM"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ApplicableItem' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ApplicableItem())
				yield return new ValidationResult() { Item = this, IssueSource = "ApplicableItem", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
