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
	public partial class IfcSurfaceOfLinearExtrusion : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSurfaceOfLinearExtrusion clause) {
			var retVal = false;
			if (clause == Where.IfcSurfaceOfLinearExtrusion.DepthGreaterZero) {
				try {
					retVal = Depth > 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcSurfaceOfLinearExtrusion");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceOfLinearExtrusion.DepthGreaterZero' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcSweptSurface)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcSurfaceOfLinearExtrusion.DepthGreaterZero))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceOfLinearExtrusion.DepthGreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSurfaceOfLinearExtrusion : IfcSweptSurface
	{
		public static readonly IfcSurfaceOfLinearExtrusion DepthGreaterZero = new IfcSurfaceOfLinearExtrusion();
		protected IfcSurfaceOfLinearExtrusion() {}
	}
}
