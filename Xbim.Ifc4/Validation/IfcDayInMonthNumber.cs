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
namespace Xbim.Ifc4.DateTimeResource
{
	public partial struct IfcDayInMonthNumber 
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.DateTimeResource.IfcDayInMonthNumber");

		/// <summary>
		/// Tests the express where clause ValidRange
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidRange() {
			var retVal = false;
			try {
				retVal = ((1 <= this) && (this <= 31) );
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidRange'.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidRange())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidRange", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
