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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcBoundaryCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcBoundaryCurve");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBoundaryCurve clause) {
			var retVal = false;
			if (clause == Where.IfcBoundaryCurve.IsClosed) {
				try {
					retVal = this/* as IfcCompositeCurve*/.ClosedCurve.AsBool();
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBoundaryCurve.IsClosed' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcCompositeCurveOnSurface)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcBoundaryCurve.IsClosed))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBoundaryCurve.IsClosed", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcBoundaryCurve : IfcCompositeCurveOnSurface
	{
		public static readonly IfcBoundaryCurve IsClosed = new IfcBoundaryCurve();
		protected IfcBoundaryCurve() {}
	}
}
