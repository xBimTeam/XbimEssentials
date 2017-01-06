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
	public partial class IfcStructuralResultGroup : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralResultGroup");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralResultGroup clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralResultGroup.HasObjectType) {
				try {
					retVal = (TheoryType != IfcAnalysisTheoryTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStructuralResultGroup.HasObjectType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralResultGroup.HasObjectType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralResultGroup.HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralResultGroup : IfcObject
	{
		public static readonly IfcStructuralResultGroup HasObjectType = new IfcStructuralResultGroup();
		protected IfcStructuralResultGroup() {}
	}
}
