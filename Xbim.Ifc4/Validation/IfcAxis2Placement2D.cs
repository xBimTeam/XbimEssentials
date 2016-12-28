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
	public partial class IfcAxis2Placement2D : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcAxis2Placement2D");

		/// <summary>
		/// Tests the express where clause RefDirIs2D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool RefDirIs2D() {
			var retVal = false;
			try {
				retVal = (!(EXISTS(RefDirection))) || (RefDirection.Dim == 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'RefDirIs2D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause LocationIs2D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool LocationIs2D() {
			var retVal = false;
			try {
				retVal = this/* as IfcPlacement*/.Location.Dim == 2;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'LocationIs2D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!RefDirIs2D())
				yield return new ValidationResult() { Item = this, IssueSource = "RefDirIs2D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!LocationIs2D())
				yield return new ValidationResult() { Item = this, IssueSource = "LocationIs2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
