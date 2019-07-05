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
	public partial class IfcCompositeCurveOnSurface : IExpressValidatable
	{
		public enum IfcCompositeCurveOnSurfaceClause
		{
			SameSurface,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCompositeCurveOnSurfaceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCompositeCurveOnSurfaceClause.SameSurface:
						retVal = Functions.SIZEOF(BasisSurface) > 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcCompositeCurveOnSurface>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCompositeCurveOnSurface.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcCompositeCurveOnSurfaceClause.SameSurface))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeCurveOnSurface.SameSurface", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
