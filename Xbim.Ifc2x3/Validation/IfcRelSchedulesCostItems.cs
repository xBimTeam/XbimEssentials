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
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.SharedMgmtElements.IfcRelSchedulesCostItems");

		/// <summary>
		/// Tests the express where clause WR11
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR11() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcRelAssigns*/.RelatedObjects.Where(temp => !(TYPEOF(temp).Contains("IFC2X3.IFCCOSTITEM")))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR11' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR12
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR12() {
			var retVal = false;
			try {
				retVal = TYPEOF(this/* as IfcRelAssignsToControl*/.RelatingControl).Contains("IFC2X3.IFCCOSTSCHEDULE");
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR12' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!WR11())
				yield return new ValidationResult() { Item = this, IssueSource = "WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR12())
				yield return new ValidationResult() { Item = this, IssueSource = "WR12", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
