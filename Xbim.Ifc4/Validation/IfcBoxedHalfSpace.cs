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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcBoxedHalfSpace : IExpressValidatable
	{
		public enum IfcBoxedHalfSpaceClause
		{
			UnboundedSurface,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcBoxedHalfSpaceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBoxedHalfSpaceClause.UnboundedSurface:
						retVal = !(Functions.TYPEOF(this/* as IfcHalfSpaceSolid*/.BaseSurface).Contains("IFC4.IFCCURVEBOUNDEDPLANE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcBoxedHalfSpace>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcBoxedHalfSpace.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcBoxedHalfSpaceClause.UnboundedSurface))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBoxedHalfSpace.UnboundedSurface", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
