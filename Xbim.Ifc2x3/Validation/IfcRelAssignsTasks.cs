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
namespace Xbim.Ifc2x3.ProcessExtension
{
	public partial class IfcRelAssignsTasks : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelAssignsTasks clause) {
			var retVal = false;
			if (clause == Where.IfcRelAssignsTasks.WR1) {
				try {
					retVal = HIINDEX(this/* as IfcRelAssigns*/.RelatedObjects) == 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcRelAssignsTasks");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelAssignsTasks.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRelAssignsTasks.WR2) {
				try {
					retVal = TYPEOF(this/* as IfcRelAssigns*/.RelatedObjects.ItemAt(0)).Contains("IFC2X3.IFCTASK");
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcRelAssignsTasks");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelAssignsTasks.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRelAssignsTasks.WR3) {
				try {
					retVal = TYPEOF(this/* as IfcRelAssignsToControl*/.RelatingControl).Contains("IFC2X3.IFCWORKCONTROL");
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProcessExtension.IfcRelAssignsTasks");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelAssignsTasks.WR3' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcRelAssignsTasks.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsTasks.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRelAssignsTasks.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsTasks.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRelAssignsTasks.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsTasks.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcRelAssignsTasks : IfcRelAssignsToControl
	{
		public new static readonly IfcRelAssignsTasks WR1 = new IfcRelAssignsTasks();
		public static readonly IfcRelAssignsTasks WR2 = new IfcRelAssignsTasks();
		public static readonly IfcRelAssignsTasks WR3 = new IfcRelAssignsTasks();
		protected IfcRelAssignsTasks() {}
	}
}
