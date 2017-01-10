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
		public enum IfcStyledItemClause
		{
			WR11,
			WR12,
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
					case IfcStyledItemClause.WR11:
						retVal = SIZEOF(Styles) == 1;
						break;
					case IfcStyledItemClause.WR12:
						retVal = !(TYPEOF(Item).Contains("IFC2X3.IFCSTYLEDITEM"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationAppearanceResource.IfcStyledItem");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStyledItem.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcStyledItemClause.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStyledItem.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcStyledItemClause.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStyledItem.WR12", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
