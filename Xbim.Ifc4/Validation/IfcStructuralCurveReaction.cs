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
namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	public partial class IfcStructuralCurveReaction : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralCurveReaction clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralCurveReaction.HasObjectType) {
				try {
					retVal = (PredefinedType != IfcStructuralCurveActivityTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralCurveReaction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralCurveReaction.HasObjectType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralCurveReaction.SuitablePredefinedType) {
				try {
					retVal = (PredefinedType != IfcStructuralCurveActivityTypeEnum.SINUS) && (PredefinedType != IfcStructuralCurveActivityTypeEnum.PARABOLA);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralCurveReaction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralCurveReaction.SuitablePredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralCurveReaction.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveReaction.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralCurveReaction.SuitablePredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveReaction.SuitablePredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralCurveReaction : IfcProduct
	{
		public static readonly IfcStructuralCurveReaction HasObjectType = new IfcStructuralCurveReaction();
		public static readonly IfcStructuralCurveReaction SuitablePredefinedType = new IfcStructuralCurveReaction();
		protected IfcStructuralCurveReaction() {}
	}
}
