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
	public partial class IfcIndexedPolyCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcIndexedPolyCurve");

		/// <summary>
		/// Tests the express where clause Consecutive
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool Consecutive() {
			var retVal = false;
			try {
				retVal = (SIZEOF(Segments) == 0) || IfcConsecutiveSegments(Segments);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'Consecutive' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!Consecutive())
				yield return new ValidationResult() { Item = this, IssueSource = "Consecutive", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
