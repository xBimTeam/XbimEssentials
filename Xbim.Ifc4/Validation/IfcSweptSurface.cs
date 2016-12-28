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
	public partial class IfcSweptSurface : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcSweptSurface");

		/// <summary>
		/// Tests the express where clause SweptCurveType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SweptCurveType() {
			var retVal = false;
			try {
				retVal = SweptCurve.ProfileType == IfcProfileTypeEnum.CURVE;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SweptCurveType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!SweptCurveType())
				yield return new ValidationResult() { Item = this, IssueSource = "SweptCurveType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
