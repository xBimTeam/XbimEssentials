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
	public partial struct IfcPHMeasure : IExpressValidatable
	{
		public enum IfcPHMeasureClause
		{
			WR21,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPHMeasureClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPHMeasureClause.WR21:
						retVal = ((0 <= this) && (this <= 14) );
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.MeasureResource.IfcPHMeasure>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPHMeasure.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPHMeasureClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPHMeasure.WR21", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
