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
	public partial class IfcStructuralSurfaceReaction : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralSurfaceReaction clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralSurfaceReaction.HasPredefinedType) {
				try {
					retVal = (PredefinedType != IfcStructuralSurfaceActivityTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralSurfaceReaction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSurfaceReaction.HasPredefinedType' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcStructuralSurfaceReaction.HasPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSurfaceReaction.HasPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralSurfaceReaction : IfcProduct
	{
		public static readonly IfcStructuralSurfaceReaction HasPredefinedType = new IfcStructuralSurfaceReaction();
		protected IfcStructuralSurfaceReaction() {}
	}
}
