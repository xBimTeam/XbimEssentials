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

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelConnectsPathElements clause) {
			var retVal = false;
			if (clause == Where.IfcRelConnectsPathElements.NormalizedRelatingPriorities) {
				try {
					retVal = (SIZEOF(RelatingPriorities) == 0) || (SIZEOF(RelatingPriorities.Where(temp => ((0 <= temp) && (temp <= 100) ))) == SIZEOF(RelatingPriorities));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcRelConnectsPathElements");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelConnectsPathElements.NormalizedRelatingPriorities' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRelConnectsPathElements.NormalizedRelatedPriorities) {
				try {
					retVal = (SIZEOF(RelatedPriorities) == 0) || (SIZEOF(RelatedPriorities.Where(temp => ((0 <= temp) && (temp <= 100) ))) == SIZEOF(RelatedPriorities));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcRelConnectsPathElements");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRelConnectsPathElements.NormalizedRelatedPriorities' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcRelConnectsElements)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcRelConnectsPathElements.NormalizedRelatingPriorities))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelConnectsPathElements.NormalizedRelatingPriorities", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRelConnectsPathElements.NormalizedRelatedPriorities))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelConnectsPathElements.NormalizedRelatedPriorities", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRelConnectsPathElements : IfcRelConnectsElements
	{
		public static readonly IfcRelConnectsPathElements NormalizedRelatingPriorities = new IfcRelConnectsPathElements();
		public static readonly IfcRelConnectsPathElements NormalizedRelatedPriorities = new IfcRelConnectsPathElements();
		protected IfcRelConnectsPathElements() {}
	}
}
