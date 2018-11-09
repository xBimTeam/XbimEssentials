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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcIndexedPolyCurve : IExpressValidatable
	{
		public enum IfcIndexedPolyCurveClause
		{
			Consecutive,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcIndexedPolyCurveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcIndexedPolyCurveClause.Consecutive:
						retVal = (Functions.SIZEOF(Segments) == 0) || Functions.IfcConsecutiveSegments(Segments);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcIndexedPolyCurve>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcIndexedPolyCurve.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcIndexedPolyCurveClause.Consecutive))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIndexedPolyCurve.Consecutive", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
