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
	public partial class IfcBoxedHalfSpace : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcBoxedHalfSpace");

		/// <summary>
		/// Tests the express where clause UnboundedSurface
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool UnboundedSurface() {
			var retVal = false;
			try {
				retVal = !(TYPEOF(this/* as IfcHalfSpaceSolid*/.BaseSurface).Contains("IFC4.IFCCURVEBOUNDEDPLANE"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'UnboundedSurface' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!UnboundedSurface())
				yield return new ValidationResult() { Item = this, IssueSource = "UnboundedSurface", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
