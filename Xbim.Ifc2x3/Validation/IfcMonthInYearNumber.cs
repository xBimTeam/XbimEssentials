using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.DateTimeResource
{
	public partial struct IfcMonthInYearNumber : IExpressValidatable
	{
		public enum IfcMonthInYearNumberClause
		{
			WR1,
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
					case IfcMonthInYearNumberClause.WR1:
						retVal = ((1 <= this) && (this <= 12) );
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.DateTimeResource.IfcMonthInYearNumber>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcMonthInYearNumber.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcMonthInYearNumberClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMonthInYearNumber.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
