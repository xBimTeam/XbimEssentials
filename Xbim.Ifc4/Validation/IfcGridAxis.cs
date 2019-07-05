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
namespace Xbim.Ifc4.GeometricConstraintResource
{
	public partial class IfcGridAxis : IExpressValidatable
	{
		public enum IfcGridAxisClause
		{
			WR1,
			WR2,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcGridAxisClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcGridAxisClause.WR1:
						retVal = AxisCurve.Dim == 2;
						break;
					case IfcGridAxisClause.WR2:
						retVal = (Functions.SIZEOF(PartOfU) == 1) ^ (Functions.SIZEOF(PartOfV) == 1) ^ (Functions.SIZEOF(PartOfW) == 1);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricConstraintResource.IfcGridAxis>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcGridAxis.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcGridAxisClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGridAxis.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcGridAxisClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGridAxis.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
