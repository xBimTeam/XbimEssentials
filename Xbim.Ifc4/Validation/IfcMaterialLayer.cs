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
namespace Xbim.Ifc4.MaterialResource
{
	public partial class IfcMaterialLayer : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.MaterialResource.IfcMaterialLayer");

		/// <summary>
		/// Tests the express where clause NormalizedPriority
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NormalizedPriority() {
			var retVal = false;
			try {
				retVal = !(EXISTS(Priority)) || ((0 <= Priority) && (Priority <= 100) );
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NormalizedPriority' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!NormalizedPriority())
				yield return new ValidationResult() { Item = this, IssueSource = "NormalizedPriority", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
