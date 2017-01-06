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
	public partial class IfcStructuralPointAction : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralPointAction");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralPointAction clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralPointAction.SuitableLoadType) {
				try {
					retVal = SIZEOF(NewArray("IFC4.IFCSTRUCTURALLOADSINGLEFORCE", "IFC4.IFCSTRUCTURALLOADSINGLEDISPLACEMENT") * TYPEOF(this/* as IfcStructuralActivity*/.AppliedLoad)) == 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStructuralPointAction.SuitableLoadType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralPointAction.SuitableLoadType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralPointAction.SuitableLoadType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStructuralPointAction : IfcProduct
	{
		public static readonly IfcStructuralPointAction SuitableLoadType = new IfcStructuralPointAction();
		protected IfcStructuralPointAction() {}
	}
}
