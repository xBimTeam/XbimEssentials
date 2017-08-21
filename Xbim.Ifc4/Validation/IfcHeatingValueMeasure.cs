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
namespace Xbim.Ifc4.MeasureResource
{
	public partial struct IfcHeatingValueMeasure : IExpressValidatable
	{
		public enum IfcHeatingValueMeasureClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcHeatingValueMeasureClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcHeatingValueMeasureClause.WR1:
						retVal = this > 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.MeasureResource.IfcHeatingValueMeasure>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcHeatingValueMeasure.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcHeatingValueMeasureClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcHeatingValueMeasure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
