using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
	public partial class IfcStructuralPlanarAction : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.StructuralAnalysisDomain.IfcStructuralPlanarAction");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralPlanarAction clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralPlanarAction.WR61) {
				try {
					retVal = SIZEOF(NewArray("IFC2X3.IFCSTRUCTURALLOADPLANARFORCE", "IFC2X3.IFCSTRUCTURALLOADTEMPERATURE") * TYPEOF(this/* as IfcStructuralActivity*/.AppliedLoad)) == 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStructuralPlanarAction.WR61' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcStructuralPlanarAction.WR61))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralPlanarAction.WR61", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcStructuralPlanarAction : IfcProduct
	{
		public static readonly IfcStructuralPlanarAction WR61 = new IfcStructuralPlanarAction();
		protected IfcStructuralPlanarAction() {}
	}
}
