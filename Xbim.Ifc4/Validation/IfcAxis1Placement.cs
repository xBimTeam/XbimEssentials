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
	public partial class IfcAxis1Placement : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis1Placement");

		/// <summary>
		/// Tests the express where clause AxisIs3D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool AxisIs3D() {
			var retVal = false;
			try {
				retVal = (!(EXISTS(Axis))) || (Axis.Dim == 3);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'AxisIs3D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause LocationIs3D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool LocationIs3D() {
			var retVal = false;
			try {
				retVal = this/* as IfcPlacement*/.Location.Dim == 3;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'LocationIs3D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!AxisIs3D())
				yield return new ValidationResult() { Item = this, IssueSource = "AxisIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!LocationIs3D())
				yield return new ValidationResult() { Item = this, IssueSource = "LocationIs3D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
