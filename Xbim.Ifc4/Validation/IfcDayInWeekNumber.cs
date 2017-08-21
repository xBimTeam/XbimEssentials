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
	public partial struct IfcDayInWeekNumber : IExpressValidatable
	{
		public enum IfcDayInWeekNumberClause
		{
			ValidRange,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDayInWeekNumberClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDayInWeekNumberClause.ValidRange:
						retVal = ((1 <= this) && (this <= 7) );
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.DateTimeResource.IfcDayInWeekNumber>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcDayInWeekNumber.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDayInWeekNumberClause.ValidRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDayInWeekNumber.ValidRange", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
