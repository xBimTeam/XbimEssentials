using System;
using log4net;
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
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial class IfcStyledItem : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationAppearanceResource.IfcStyledItem");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStyledItem clause) {
			var retVal = false;
			if (clause == Where.IfcStyledItem.WR11) {
				try {
					retVal = SIZEOF(Styles) == 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStyledItem.WR11' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStyledItem.WR12) {
				try {
					retVal = !(TYPEOF(Item).Contains("IFC2X3.IFCSTYLEDITEM"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStyledItem.WR12' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcStyledItem.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStyledItem.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStyledItem.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStyledItem.WR12", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcStyledItem
	{
		public static readonly IfcStyledItem WR11 = new IfcStyledItem();
		public static readonly IfcStyledItem WR12 = new IfcStyledItem();
		protected IfcStyledItem() {}
	}
}
