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
		/// Tests the express where clause NoSurfaces
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoSurfaces() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcGeometricSet*/.Elements.Where(Temp => TYPEOF(Temp).Contains("IFC4.IFCSURFACE"))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoSurfaces' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!NoSurfaces())
				yield return new ValidationResult() { Item = this, IssueSource = "NoSurfaces", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
