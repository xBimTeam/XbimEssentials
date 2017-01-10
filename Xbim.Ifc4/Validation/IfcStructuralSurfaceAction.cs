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
	public partial class IfcStructuralSurfaceAction : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralSurfaceAction clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralSurfaceAction.ProjectedIsGlobal) {
				try {
					retVal = (!EXISTS(ProjectedOrTrue)) || ((ProjectedOrTrue != IfcProjectedOrTrueLengthEnum.PROJECTED_LENGTH) || (this/* as IfcStructuralActivity*/.GlobalOrLocal == IfcGlobalOrLocalEnum.GLOBAL_COORDS));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralSurfaceAction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSurfaceAction.ProjectedIsGlobal' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralSurfaceAction.HasObjectType) {
				try {
					retVal = (PredefinedType != IfcStructuralSurfaceActivityTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralSurfaceAction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSurfaceAction.HasObjectType' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcStructuralSurfaceAction.ProjectedIsGlobal))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceAction.ProjectedIsGlobal", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralSurfaceAction.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceAction.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralSurfaceAction : IfcProduct
	{
		public static readonly IfcStructuralSurfaceAction ProjectedIsGlobal = new IfcStructuralSurfaceAction();
		public static readonly IfcStructuralSurfaceAction HasObjectType = new IfcStructuralSurfaceAction();
		protected IfcStructuralSurfaceAction() {}
	}
}
