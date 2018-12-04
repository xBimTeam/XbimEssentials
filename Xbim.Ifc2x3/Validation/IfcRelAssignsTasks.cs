using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.ProcessExtension
{
	public partial class IfcRelAssignsTasks : IExpressValidatable
	{
		public enum IfcRelAssignsTasksClause
		{
			WR1,
			WR2,
			WR3,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelAssignsTasksClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelAssignsTasksClause.WR1:
						retVal = Functions.HIINDEX(this/* as IfcRelAssigns*/.RelatedObjects) == 1;
						break;
					case IfcRelAssignsTasksClause.WR2:
						retVal = Functions.TYPEOF(this/* as IfcRelAssigns*/.RelatedObjects.ItemAt(0)).Contains("IFC2X3.IFCTASK");
						break;
					case IfcRelAssignsTasksClause.WR3:
						retVal = Functions.TYPEOF(this/* as IfcRelAssignsToControl*/.RelatingControl).Contains("IFC2X3.IFCWORKCONTROL");
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProcessExtension.IfcRelAssignsTasks>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelAssignsTasks.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRelAssignsTasksClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsTasks.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRelAssignsTasksClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsTasks.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRelAssignsTasksClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssignsTasks.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
