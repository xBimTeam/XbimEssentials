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
	public partial class IfcCompositeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcCompositeProfileDef");

		/// <summary>
		/// Tests the express where clause InvariantProfileType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool InvariantProfileType() {
			var retVal = false;
			try {
				retVal = SIZEOF(Profiles.Where(temp => temp.ProfileType != Profiles.ToArray()[0].ProfileType)) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'InvariantProfileType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NoRecursion
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoRecursion() {
			var retVal = false;
			try {
				retVal = SIZEOF(Profiles.Where(temp => TYPEOF(temp).Contains("IFC4.IFCCOMPOSITEPROFILEDEF"))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoRecursion' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!InvariantProfileType())
				yield return new ValidationResult() { Item = this, IssueSource = "InvariantProfileType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NoRecursion())
				yield return new ValidationResult() { Item = this, IssueSource = "NoRecursion", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
