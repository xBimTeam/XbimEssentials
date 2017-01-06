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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStyledItem clause) {
			var retVal = false;
			if (clause == Where.IfcStyledItem.ApplicableItem) {
				try {
					retVal = !(TYPEOF(Item).Contains("IFC4.IFCSTYLEDITEM"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStyledItem.ApplicableItem' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcStyledItem.ApplicableItem))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStyledItem.ApplicableItem", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStyledItem
	{
		public static readonly IfcStyledItem ApplicableItem = new IfcStyledItem();
		protected IfcStyledItem() {}
	}
}
