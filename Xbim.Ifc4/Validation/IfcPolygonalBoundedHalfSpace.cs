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
	public partial class IfcPolygonalBoundedHalfSpace : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcPolygonalBoundedHalfSpace");

		/// <summary>
		/// Tests the express where clause BoundaryDim
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool BoundaryDim() {
			var retVal = false;
			try {
				retVal = PolygonalBoundary.Dim == 2;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'BoundaryDim' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause BoundaryType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool BoundaryType() {
			var retVal = false;
			try {
				retVal = SIZEOF(TYPEOF(PolygonalBoundary) * NewArray("IFC4.IFCPOLYLINE", "IFC4.IFCCOMPOSITECURVE")) == 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'BoundaryType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!BoundaryDim())
				yield return new ValidationResult() { Item = this, IssueSource = "BoundaryDim", IssueType = ValidationFlags.EntityWhereClauses };
			if (!BoundaryType())
				yield return new ValidationResult() { Item = this, IssueSource = "BoundaryType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
