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
	public partial struct IfcCardinalPointReference 
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.MaterialResource.IfcCardinalPointReference");

		/// <summary>
		/// Tests the express where clause GreaterThanZero
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool GreaterThanZero() {
			var retVal = false;
			try {
				retVal = this > 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'GreaterThanZero'.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!GreaterThanZero())
				yield return new ValidationResult() { Item = this, IssueSource = "GreaterThanZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
