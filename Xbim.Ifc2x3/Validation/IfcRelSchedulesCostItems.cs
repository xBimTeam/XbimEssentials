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
namespace Xbim.Ifc2x3.SharedMgmtElements
{
	public partial class IfcRelSchedulesCostItems : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelSchedulesCostItems clause) {
			var retVal = false;
			if (clause == Where.IfcRelSchedulesCostItems.WR11) {
				try {
					retVal = SIZEOF(this/* as IfcRelAssigns*/.RelatedObjects.Where(temp => !(TYPEOF(temp).Contains("IFC2X3.IFCCOSTITEM")))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.SharedMgmtElements.IfcRelSchedulesCostItems");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelSchedulesCostItems.WR11' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRelSchedulesCostItems.WR12) {
				try {
					retVal = TYPEOF(this/* as IfcRelAssignsToControl*/.RelatingControl).Contains("IFC2X3.IFCCOSTSCHEDULE");
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.SharedMgmtElements.IfcRelSchedulesCostItems");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelSchedulesCostItems.WR12' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcRelAssignsToControl)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcRelSchedulesCostItems.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSchedulesCostItems.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRelSchedulesCostItems.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSchedulesCostItems.WR12", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcRelSchedulesCostItems : IfcRelAssignsToControl
	{
		public static readonly IfcRelSchedulesCostItems WR11 = new IfcRelSchedulesCostItems();
		public static readonly IfcRelSchedulesCostItems WR12 = new IfcRelSchedulesCostItems();
		protected IfcRelSchedulesCostItems() {}
	}
}
