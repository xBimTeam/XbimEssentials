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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcSurfaceCurveSweptAreaSolid : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSurfaceCurveSweptAreaSolid clause) {
			var retVal = false;
			if (clause == Where.IfcSurfaceCurveSweptAreaSolid.DirectrixBounded) {
				try {
					retVal = (EXISTS(StartParam) && EXISTS(EndParam)) || (SIZEOF(NewArray("IFC4.IFCCONIC", "IFC4.IFCBOUNDEDCURVE") * TYPEOF(Directrix)) == 1);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcSurfaceCurveSweptAreaSolid");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceCurveSweptAreaSolid.DirectrixBounded' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcSweptAreaSolid)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcSurfaceCurveSweptAreaSolid.DirectrixBounded))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceCurveSweptAreaSolid.DirectrixBounded", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSurfaceCurveSweptAreaSolid : IfcSweptAreaSolid
	{
		public static readonly IfcSurfaceCurveSweptAreaSolid DirectrixBounded = new IfcSurfaceCurveSweptAreaSolid();
		protected IfcSurfaceCurveSweptAreaSolid() {}
	}
}
