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
namespace Xbim.Ifc2x3.SharedMgmtElements
{
	public partial class IfcRelSchedulesCostItems : IExpressValidatable
	{
		public enum IfcRelSchedulesCostItemsClause
		{
			WR11,
			WR12,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelSchedulesCostItemsClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelSchedulesCostItemsClause.WR11:
						retVal = Functions.SIZEOF(this/* as IfcRelAssigns*/.RelatedObjects.Where(temp => !(Functions.TYPEOF(temp).Contains("IFC2X3.IFCCOSTITEM")))) == 0;
						break;
					case IfcRelSchedulesCostItemsClause.WR12:
						retVal = Functions.TYPEOF(this/* as IfcRelAssignsToControl*/.RelatingControl).Contains("IFC2X3.IFCCOSTSCHEDULE");
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.SharedMgmtElements.IfcRelSchedulesCostItems>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelSchedulesCostItems.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRelSchedulesCostItemsClause.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSchedulesCostItems.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRelSchedulesCostItemsClause.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSchedulesCostItems.WR12", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
