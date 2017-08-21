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
	public partial struct IfcMonthInYearNumber : IExpressValidatable
	{
		public enum IfcMonthInYearNumberClause
		{
			ValidRange,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcMonthInYearNumberClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcMonthInYearNumberClause.ValidRange:
						retVal = ((1 <= this) && (this <= 12) );
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.DateTimeResource.IfcMonthInYearNumber>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcMonthInYearNumber.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcMonthInYearNumberClause.ValidRange))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMonthInYearNumber.ValidRange", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
