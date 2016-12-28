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
namespace Xbim.Ifc4.ArchitectureDomain
{
	public partial class IfcDoorLiningProperties : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ArchitectureDomain.IfcDoorLiningProperties");

		/// <summary>
		/// Tests the express where clause WR31
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR31() {
			var retVal = false;
			try {
				retVal = !(EXISTS(LiningDepth) && !(EXISTS(LiningThickness)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR31' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR32
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR32() {
			var retVal = false;
			try {
				retVal = !(EXISTS(ThresholdDepth) && !(EXISTS(ThresholdThickness)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR32' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR33
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR33() {
			var retVal = false;
			try {
				retVal = (EXISTS(TransomOffset) && EXISTS(TransomThickness)) ^ (!(EXISTS(TransomOffset)) && !(EXISTS(TransomThickness)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR33' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR34
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR34() {
			var retVal = false;
			try {
				retVal = (EXISTS(CasingDepth) && EXISTS(CasingThickness)) ^ (!(EXISTS(CasingDepth)) && !(EXISTS(CasingThickness)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR34' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause WR35
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR35() {
			var retVal = false;
			try {
				retVal = (EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0])) && ((TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0]).Contains("IFC4.IFCDOORTYPE")) || (TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0]).Contains("IFC4.IFCDOORSTYLE")));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR35' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR31())
				yield return new ValidationResult() { Item = this, IssueSource = "WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR32())
				yield return new ValidationResult() { Item = this, IssueSource = "WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR33())
				yield return new ValidationResult() { Item = this, IssueSource = "WR33", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR34())
				yield return new ValidationResult() { Item = this, IssueSource = "WR34", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR35())
				yield return new ValidationResult() { Item = this, IssueSource = "WR35", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
