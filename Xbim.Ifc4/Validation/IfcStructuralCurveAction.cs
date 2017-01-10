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
	public partial class IfcStructuralCurveAction : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralCurveAction clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralCurveAction.ProjectedIsGlobal) {
				try {
					retVal = (!EXISTS(ProjectedOrTrue)) || ((ProjectedOrTrue != IfcProjectedOrTrueLengthEnum.PROJECTED_LENGTH) || (this/* as IfcStructuralActivity*/.GlobalOrLocal == IfcGlobalOrLocalEnum.GLOBAL_COORDS));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralCurveAction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralCurveAction.ProjectedIsGlobal' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralCurveAction.HasObjectType) {
				try {
					retVal = (PredefinedType != IfcStructuralCurveActivityTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralCurveAction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralCurveAction.HasObjectType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralCurveAction.SuitablePredefinedType) {
				try {
					retVal = PredefinedType != IfcStructuralCurveActivityTypeEnum.EQUIDISTANT;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralCurveAction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralCurveAction.SuitablePredefinedType' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcStructuralCurveAction.ProjectedIsGlobal))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveAction.ProjectedIsGlobal", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralCurveAction.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveAction.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralCurveAction.SuitablePredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralCurveAction.SuitablePredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralCurveAction : IfcProduct
	{
		public static readonly IfcStructuralCurveAction ProjectedIsGlobal = new IfcStructuralCurveAction();
		public static readonly IfcStructuralCurveAction HasObjectType = new IfcStructuralCurveAction();
		public static readonly IfcStructuralCurveAction SuitablePredefinedType = new IfcStructuralCurveAction();
		protected IfcStructuralCurveAction() {}
	}
}
