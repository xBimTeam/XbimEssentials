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
	public partial class IfcSweptSurface : IExpressValidatable
	{
		public enum IfcSweptSurfaceClause
		{
			SweptCurveType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSweptSurfaceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSweptSurfaceClause.SweptCurveType:
						retVal = SweptCurve.ProfileType == IfcProfileTypeEnum.CURVE;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcSweptSurface>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSweptSurface.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcSweptSurfaceClause.SweptCurveType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptSurface.SweptCurveType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
