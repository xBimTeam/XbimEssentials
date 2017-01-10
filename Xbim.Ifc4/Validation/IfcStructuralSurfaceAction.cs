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
		public enum IfcStructuralSurfaceActionClause
		{
			ProjectedIsGlobal,
			HasObjectType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralSurfaceActionClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralSurfaceActionClause.ProjectedIsGlobal:
						retVal = (!EXISTS(ProjectedOrTrue)) || ((ProjectedOrTrue != IfcProjectedOrTrueLengthEnum.PROJECTED_LENGTH) || (this/* as IfcStructuralActivity*/.GlobalOrLocal == IfcGlobalOrLocalEnum.GLOBAL_COORDS));
						break;
					case IfcStructuralSurfaceActionClause.HasObjectType:
						retVal = (PredefinedType != IfcStructuralSurfaceActivityTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralSurfaceAction");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSurfaceAction.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralSurfaceActionClause.ProjectedIsGlobal))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceAction.ProjectedIsGlobal", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcStructuralSurfaceActionClause.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceAction.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
