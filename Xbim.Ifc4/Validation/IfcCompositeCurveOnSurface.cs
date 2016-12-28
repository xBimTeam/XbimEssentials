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
	public partial class IfcCompositeCurveOnSurface : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCompositeCurveOnSurface");

		/// <summary>
		/// Tests the express where clause SameSurface
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SameSurface() {
			var retVal = false;
			try {
				retVal = SIZEOF(BasisSurface) > 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SameSurface' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!SameSurface())
				yield return new ValidationResult() { Item = this, IssueSource = "SameSurface", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
