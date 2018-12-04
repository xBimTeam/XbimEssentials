using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.DateTimeResource
{
	public partial struct IfcDayInMonthNumber : IExpressValidatable
	{
		public enum IfcDayInMonthNumberClause
		{
			ValidRange,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDayInMonthNumberClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDayInMonthNumberClause.ValidRange:
						retVal = ((1 <= this) && (this <= 31) );
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.DateTimeResource.IfcDayInMonthNumber>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcDayInMonthNumber.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDayInMonthNumberClause.ValidRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDayInMonthNumber.ValidRange", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
