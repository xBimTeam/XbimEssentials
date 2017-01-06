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
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class Ifc2DCompositeCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.Ifc2DCompositeCurve");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.Ifc2DCompositeCurve clause) {
			var retVal = false;
			if (clause == Where.Ifc2DCompositeCurve.WR1) {
				try {
					retVal = this/* as IfcCompositeCurve*/.ClosedCurve.Value;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'Ifc2DCompositeCurve.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.Ifc2DCompositeCurve.WR2) {
				try {
					retVal = this/* as IfcCurve*/.Dim == 2;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'Ifc2DCompositeCurve.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcCompositeCurve)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.Ifc2DCompositeCurve.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "Ifc2DCompositeCurve.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.Ifc2DCompositeCurve.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "Ifc2DCompositeCurve.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class Ifc2DCompositeCurve : IfcCompositeCurve
	{
		public static readonly Ifc2DCompositeCurve WR1 = new Ifc2DCompositeCurve();
		public static readonly Ifc2DCompositeCurve WR2 = new Ifc2DCompositeCurve();
		protected Ifc2DCompositeCurve() {}
	}
}
