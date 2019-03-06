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
namespace Xbim.IfcRail.ProductExtension
{
	public partial class IfcRelSpaceBoundary : IExpressValidatable
	{
		public enum IfcRelSpaceBoundaryClause
		{
			CorrectPhysOrVirt,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelSpaceBoundaryClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelSpaceBoundaryClause.CorrectPhysOrVirt:
						retVal = ((PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.PHYSICAL) && (!(Functions.TYPEOF(RelatedBuildingElement).Contains("IFCVIRTUALELEMENT")))) || ((PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.VIRTUAL) && ((Functions.TYPEOF(RelatedBuildingElement).Contains("IFCVIRTUALELEMENT")) || (Functions.TYPEOF(RelatedBuildingElement).Contains("IFCOPENINGELEMENT")))) || (PhysicalOrVirtualBoundary == IfcPhysicalOrVirtualEnum.NOTDEFINED);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.IfcRail.ProductExtension.IfcRelSpaceBoundary>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelSpaceBoundary.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRelSpaceBoundaryClause.CorrectPhysOrVirt))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSpaceBoundary.CorrectPhysOrVirt", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
