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
	public partial class IfcOffsetCurve3D : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcOffsetCurve3D");

		/// <summary>
		/// Tests the express where clause DimIs2D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool DimIs2D() {
			var retVal = false;
			try {
				retVal = BasisCurve.Dim == 3;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'DimIs2D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!DimIs2D())
				yield return new ValidationResult() { Item = this, IssueSource = "DimIs2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
