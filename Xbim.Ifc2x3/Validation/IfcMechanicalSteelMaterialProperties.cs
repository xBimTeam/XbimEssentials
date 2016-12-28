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
namespace Xbim.Ifc2x3.MaterialPropertyResource
{
	public partial class IfcMechanicalSteelMaterialProperties : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MaterialPropertyResource.IfcMechanicalSteelMaterialProperties");

		/// <summary>
		/// Tests the express where clause WR31
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR31() {
			var retVal = false;
			try {
				retVal = !(EXISTS(YieldStress)) || (YieldStress >= 0);
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
				retVal = !(EXISTS(UltimateStress)) || (UltimateStress >= 0);
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
				retVal = !(EXISTS(HardeningModule)) || (HardeningModule >= 0);
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
				retVal = !(EXISTS(ProportionalStress)) || (ProportionalStress >= 0);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR34' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!WR31())
				yield return new ValidationResult() { Item = this, IssueSource = "WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR32())
				yield return new ValidationResult() { Item = this, IssueSource = "WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR33())
				yield return new ValidationResult() { Item = this, IssueSource = "WR33", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR34())
				yield return new ValidationResult() { Item = this, IssueSource = "WR34", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
