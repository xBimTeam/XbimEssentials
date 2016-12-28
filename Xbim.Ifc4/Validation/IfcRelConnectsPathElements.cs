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
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcRelConnectsPathElements : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcRelConnectsPathElements");

		/// <summary>
		/// Tests the express where clause NormalizedRelatingPriorities
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NormalizedRelatingPriorities() {
			var retVal = false;
			try {
				retVal = (SIZEOF(RelatingPriorities) == 0) || (SIZEOF(RelatingPriorities.Where(temp => ((0 <= temp) && (temp <= 100) ))) == SIZEOF(RelatingPriorities));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NormalizedRelatingPriorities' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause NormalizedRelatedPriorities
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NormalizedRelatedPriorities() {
			var retVal = false;
			try {
				retVal = (SIZEOF(RelatedPriorities) == 0) || (SIZEOF(RelatedPriorities.Where(temp => ((0 <= temp) && (temp <= 100) ))) == SIZEOF(RelatedPriorities));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NormalizedRelatedPriorities' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!NormalizedRelatingPriorities())
				yield return new ValidationResult() { Item = this, IssueSource = "NormalizedRelatingPriorities", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NormalizedRelatedPriorities())
				yield return new ValidationResult() { Item = this, IssueSource = "NormalizedRelatedPriorities", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
