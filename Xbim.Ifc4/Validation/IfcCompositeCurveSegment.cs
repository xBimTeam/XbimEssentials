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
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCompositeCurveSegment");

		/// <summary>
		/// Tests the express where clause ParentIsBoundedCurve
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ParentIsBoundedCurve() {
			var retVal = false;
			try {
				retVal = (TYPEOF(ParentCurve).Contains("IFC4.IFCBOUNDEDCURVE"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ParentIsBoundedCurve' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ParentIsBoundedCurve())
				yield return new ValidationResult() { Item = this, IssueSource = "ParentIsBoundedCurve", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
