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
namespace Xbim.Ifc4.PresentationOrganizationResource
{
	public partial class IfcPresentationLayerWithStyle : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationOrganizationResource.IfcPresentationLayerWithStyle");

		/// <summary>
		/// Tests the express where clause ApplicableOnlyToItems
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ApplicableOnlyToItems() {
			var retVal = false;
			try {
				retVal = SIZEOF(AssignedItems.Where(temp => (SIZEOF(TYPEOF(temp) * NewArray("IFC4.IFCGEOMETRICREPRESENTATIONITEM", "IFC4.IFCMAPPEDITEM")) == 1))) == SIZEOF(AssignedItems);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ApplicableOnlyToItems' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ApplicableOnlyToItems())
				yield return new ValidationResult() { Item = this, IssueSource = "ApplicableOnlyToItems", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
