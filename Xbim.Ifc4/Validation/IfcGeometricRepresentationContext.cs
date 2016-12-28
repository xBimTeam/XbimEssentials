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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcGeometricRepresentationContext : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcGeometricRepresentationContext");

		/// <summary>
		/// Tests the express where clause North2D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool North2D() {
			var retVal = false;
			try {
				retVal = !(EXISTS(TrueNorth)) || (HIINDEX(TrueNorth.DirectionRatios) == 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'North2D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!North2D())
				yield return new ValidationResult() { Item = this, IssueSource = "North2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
