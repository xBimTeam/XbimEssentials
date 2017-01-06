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
	public partial class IfcStructuralLinearAction : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralLinearAction");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralLinearAction clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralLinearAction.SuitableLoadType) {
				try {
					retVal = SIZEOF(NewArray("IFC4.IFCSTRUCTURALLOADLINEARFORCE", "IFC4.IFCSTRUCTURALLOADTEMPERATURE") * TYPEOF(this/* as IfcStructuralActivity*/.AppliedLoad)) == 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStructuralLinearAction.SuitableLoadType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralLinearAction.ConstPredefinedType) {
				try {
					retVal = this/* as IfcStructuralCurveAction*/.PredefinedType == IfcStructuralCurveActivityTypeEnum.CONST;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStructuralLinearAction.ConstPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcStructuralCurveAction)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralLinearAction.SuitableLoadType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralLinearAction.SuitableLoadType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralLinearAction.ConstPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralLinearAction.ConstPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralLinearAction : IfcStructuralCurveAction
	{
		public static readonly IfcStructuralLinearAction SuitableLoadType = new IfcStructuralLinearAction();
		public static readonly IfcStructuralLinearAction ConstPredefinedType = new IfcStructuralLinearAction();
		protected IfcStructuralLinearAction() {}
	}
}
