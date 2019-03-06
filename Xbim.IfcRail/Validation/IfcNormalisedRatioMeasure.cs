using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.IfcRail.MeasureResource
{
	public partial struct IfcNormalisedRatioMeasure : IExpressValidatable
	{
		public enum IfcNormalisedRatioMeasureClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcNormalisedRatioMeasureClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcNormalisedRatioMeasureClause.WR1:
						retVal = ((0 <= this) && (this <= 1) );
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.IfcRail.MeasureResource.IfcNormalisedRatioMeasure>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcNormalisedRatioMeasure.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcNormalisedRatioMeasureClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcNormalisedRatioMeasure.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
