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
	public partial class IfcCompositeCurveSegment : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCompositeCurveSegment clause) {
			var retVal = false;
			if (clause == Where.IfcCompositeCurveSegment.ParentIsBoundedCurve) {
				try {
					retVal = (TYPEOF(ParentCurve).Contains("IFC4.IFCBOUNDEDCURVE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCompositeCurveSegment");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCompositeCurveSegment.ParentIsBoundedCurve' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCompositeCurveSegment.ParentIsBoundedCurve))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeCurveSegment.ParentIsBoundedCurve", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCompositeCurveSegment
	{
		public static readonly IfcCompositeCurveSegment ParentIsBoundedCurve = new IfcCompositeCurveSegment();
		protected IfcCompositeCurveSegment() {}
	}
}
