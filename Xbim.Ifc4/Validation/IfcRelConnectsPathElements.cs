using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
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
						retVal = (SIZEOF(RelatingPriorities) == 0) || (SIZEOF(RelatingPriorities.Where(temp => ((0 <= temp) && (temp <= 100) ))) == SIZEOF(RelatingPriorities));
						break;
					case IfcRelConnectsPathElementsClause.NormalizedRelatedPriorities:
						retVal = (SIZEOF(RelatedPriorities) == 0) || (SIZEOF(RelatedPriorities.Where(temp => ((0 <= temp) && (temp <= 100) ))) == SIZEOF(RelatedPriorities));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcRelConnectsPathElements");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelConnectsPathElements.{0}' for #{1}.", clause,EntityLabel), ex);
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
