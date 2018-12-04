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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcGeometricRepresentationContext : IExpressValidatable
	{
		public enum IfcGeometricRepresentationContextClause
		{
			North2D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcGeometricRepresentationContextClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcGeometricRepresentationContextClause.North2D:
						retVal = !(Functions.EXISTS(TrueNorth)) || (Functions.HIINDEX(TrueNorth.DirectionRatios) == 2);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.RepresentationResource.IfcGeometricRepresentationContext>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcGeometricRepresentationContext.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcGeometricRepresentationContextClause.North2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationContext.North2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
