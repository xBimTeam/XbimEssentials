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
	public partial class IfcGeometricCurveSet : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcGeometricCurveSet");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcGeometricCurveSet clause) {
			var retVal = false;
			if (clause == Where.IfcGeometricCurveSet.NoSurfaces) {
				try {
					retVal = SIZEOF(this/* as IfcGeometricSet*/.Elements.Where(Temp => TYPEOF(Temp).Contains("IFC4.IFCSURFACE"))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcGeometricCurveSet.NoSurfaces' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcGeometricSet)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcGeometricCurveSet.NoSurfaces))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricCurveSet.NoSurfaces", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcGeometricCurveSet : IfcGeometricSet
	{
		public static readonly IfcGeometricCurveSet NoSurfaces = new IfcGeometricCurveSet();
		protected IfcGeometricCurveSet() {}
	}
}
