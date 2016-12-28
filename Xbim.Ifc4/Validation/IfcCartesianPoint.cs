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
	public partial class IfcCartesianPoint : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianPoint");

		/// <summary>
		/// Tests the express where clause CP2Dor3D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CP2Dor3D() {
			var retVal = false;
			try {
				retVal = HIINDEX(Coordinates) >= 2;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CP2Dor3D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!CP2Dor3D())
				yield return new ValidationResult() { Item = this, IssueSource = "CP2Dor3D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
