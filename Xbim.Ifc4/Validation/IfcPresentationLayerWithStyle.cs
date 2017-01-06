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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPresentationLayerWithStyle clause) {
			var retVal = false;
			if (clause == Where.IfcPresentationLayerWithStyle.ApplicableOnlyToItems) {
				try {
					retVal = SIZEOF(AssignedItems.Where(temp => (SIZEOF(TYPEOF(temp) * NewArray("IFC4.IFCGEOMETRICREPRESENTATIONITEM", "IFC4.IFCMAPPEDITEM")) == 1))) == SIZEOF(AssignedItems);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPresentationLayerWithStyle.ApplicableOnlyToItems' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcPresentationLayerAssignment)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcPresentationLayerWithStyle.ApplicableOnlyToItems))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPresentationLayerWithStyle.ApplicableOnlyToItems", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPresentationLayerWithStyle : IfcPresentationLayerAssignment
	{
		public static readonly IfcPresentationLayerWithStyle ApplicableOnlyToItems = new IfcPresentationLayerWithStyle();
		protected IfcPresentationLayerWithStyle() {}
	}
}
