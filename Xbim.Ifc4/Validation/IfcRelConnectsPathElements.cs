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
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcRelConnectsPathElements : IExpressValidatable
	{
		public enum IfcRelConnectsPathElementsClause
		{
			NormalizedRelatingPriorities,
			NormalizedRelatedPriorities,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelConnectsPathElementsClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelConnectsPathElementsClause.NormalizedRelatingPriorities:
						retVal = (Functions.SIZEOF(RelatingPriorities) == 0) || (Functions.SIZEOF(RelatingPriorities.Where(temp => ((0 <= temp) && (temp <= 100) ))) == Functions.SIZEOF(RelatingPriorities));
						break;
					case IfcRelConnectsPathElementsClause.NormalizedRelatedPriorities:
						retVal = (Functions.SIZEOF(RelatedPriorities) == 0) || (Functions.SIZEOF(RelatedPriorities.Where(temp => ((0 <= temp) && (temp <= 100) ))) == Functions.SIZEOF(RelatedPriorities));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.SharedBldgElements.IfcRelConnectsPathElements>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelConnectsPathElements.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcRelConnectsPathElementsClause.NormalizedRelatingPriorities))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelConnectsPathElements.NormalizedRelatingPriorities", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRelConnectsPathElementsClause.NormalizedRelatedPriorities))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelConnectsPathElements.NormalizedRelatedPriorities", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
