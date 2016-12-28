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
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcArbitraryOpenProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcArbitraryOpenProfileDef");

		/// <summary>
		/// Tests the express where clause WR11
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR11() {
			var retVal = false;
			try {
				retVal = (TYPEOF(this).Contains("IFC4.IFCCENTERLINEPROFILEDEF")) || (this/* as IfcProfileDef*/.ProfileType == IfcProfileTypeEnum.CURVE);
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
				retVal = Curve.Dim == 2;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR12' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR11())
				yield return new ValidationResult() { Item = this, IssueSource = "WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!WR12())
				yield return new ValidationResult() { Item = this, IssueSource = "WR12", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
