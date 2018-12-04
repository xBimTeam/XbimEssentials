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
	public partial class IfcStructuralCurveAction : IExpressValidatable
	{
		public enum IfcStructuralCurveActionClause
		{
			ProjectedIsGlobal,
			HasObjectType,
			SuitablePredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralCurveActionClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralCurveActionClause.ProjectedIsGlobal:
						retVal = (!Functions.EXISTS(ProjectedOrTrue)) || ((ProjectedOrTrue != IfcProjectedOrTrueLengthEnum.PROJECTED_LENGTH) || (this/* as IfcStructuralActivity*/.GlobalOrLocal == IfcGlobalOrLocalEnum.GLOBAL_COORDS));
						break;
					case IfcStructuralCurveActionClause.HasObjectType:
						retVal = (PredefinedType != IfcStructuralCurveActivityTypeEnum.USERDEFINED) || Functions.EXISTS(this/* as IfcObject*/.ObjectType);
						break;
					case IfcStructuralCurveActionClause.SuitablePredefinedType:
						retVal = PredefinedType != IfcStructuralCurveActivityTypeEnum.EQUIDISTANT;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralCurveAction>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStructuralCurveAction.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralCurveActionClause.ProjectedIsGlobal))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveAction.ProjectedIsGlobal", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcStructuralCurveActionClause.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveAction.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcStructuralCurveActionClause.SuitablePredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveAction.SuitablePredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
