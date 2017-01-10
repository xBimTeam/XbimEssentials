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
	public partial class IfcStructuralAnalysisModel : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralAnalysisModel clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralAnalysisModel.HasObjectType) {
				try {
					retVal = (PredefinedType != IfcAnalysisModelTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralAnalysisModel");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralAnalysisModel.HasObjectType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralAnalysisModel.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralAnalysisModel.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralAnalysisModel : IfcObject
	{
		public static readonly IfcStructuralAnalysisModel HasObjectType = new IfcStructuralAnalysisModel();
		protected IfcStructuralAnalysisModel() {}
	}
}
