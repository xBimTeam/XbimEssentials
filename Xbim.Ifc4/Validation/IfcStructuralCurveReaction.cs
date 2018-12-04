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
namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	public partial class IfcStructuralCurveReaction : IExpressValidatable
	{
		public enum IfcStructuralCurveReactionClause
		{
			HasObjectType,
			SuitablePredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralCurveReactionClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralCurveReactionClause.HasObjectType:
						retVal = (PredefinedType != IfcStructuralCurveActivityTypeEnum.USERDEFINED) || Functions.EXISTS(this/* as IfcObject*/.ObjectType);
						break;
					case IfcStructuralCurveReactionClause.SuitablePredefinedType:
						retVal = (PredefinedType != IfcStructuralCurveActivityTypeEnum.SINUS) && (PredefinedType != IfcStructuralCurveActivityTypeEnum.PARABOLA);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralCurveReaction>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStructuralCurveReaction.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralCurveReactionClause.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveReaction.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcStructuralCurveReactionClause.SuitablePredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveReaction.SuitablePredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
