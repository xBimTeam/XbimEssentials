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
		public enum IfcStyledItemClause
		{
			ApplicableItem,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStyledItemClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStyledItemClause.ApplicableItem:
						retVal = !(TYPEOF(Item).Contains("IFC4.IFCSTYLEDITEM"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcStyledItem");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStyledItem.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcStyledItemClause.ApplicableItem))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStyledItem.ApplicableItem", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
