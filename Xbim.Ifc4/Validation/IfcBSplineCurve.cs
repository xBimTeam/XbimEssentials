using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.GeometryResource
{
    public partial class IfcBSplineCurve : IExpressValidatable
	{
		public enum IfcBSplineCurveClause
		{
			SameDim,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcBSplineCurveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBSplineCurveClause.SameDim:
						retVal = Functions.SIZEOF(ControlPointsList.Where(Temp => Temp.Dim != ControlPointsList.ItemAt(0).Dim)) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcBSplineCurve>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcBSplineCurve.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcBSplineCurveClause.SameDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBSplineCurve.SameDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
