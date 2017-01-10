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
	public partial class IfcStructuralPlanarAction : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralPlanarAction clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralPlanarAction.SuitableLoadType) {
				try {
					retVal = SIZEOF(NewArray("IFC4.IFCSTRUCTURALLOADPLANARFORCE", "IFC4.IFCSTRUCTURALLOADTEMPERATURE") * TYPEOF(this/* as IfcStructuralActivity*/.AppliedLoad)) == 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralPlanarAction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralPlanarAction.SuitableLoadType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralPlanarAction.ConstPredefinedType) {
				try {
					retVal = this/* as IfcStructuralSurfaceAction*/.PredefinedType == IfcStructuralSurfaceActivityTypeEnum.CONST;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralPlanarAction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralPlanarAction.ConstPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcStructuralSurfaceAction)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralPlanarAction.SuitableLoadType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralPlanarAction.SuitableLoadType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralPlanarAction.ConstPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralPlanarAction.ConstPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralPlanarAction : IfcStructuralSurfaceAction
	{
		public static readonly IfcStructuralPlanarAction SuitableLoadType = new IfcStructuralPlanarAction();
		public static readonly IfcStructuralPlanarAction ConstPredefinedType = new IfcStructuralPlanarAction();
		protected IfcStructuralPlanarAction() {}
	}
}
