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
namespace Xbim.Ifc4.PropertyResource
{
	public partial class IfcPropertyEnumeration : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PropertyResource.IfcPropertyEnumeration");

		/// <summary>
		/// Tests the express where clause WR01
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR01() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.EnumerationValues.Where(temp => !(TYPEOF(this.EnumerationValues.ToArray()[0]) == TYPEOF(temp)))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR01' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR01())
				yield return new ValidationResult() { Item = this, IssueSource = "WR01", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
