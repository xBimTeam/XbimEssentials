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
	public partial class IfcDirection : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcDirection");

		/// <summary>
		/// Tests the express where clause MagnitudeGreaterZero
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MagnitudeGreaterZero() {
			var retVal = false;
			try {
				retVal = SIZEOF(DirectionRatios.Where(Tmp => Tmp != 0)) > 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MagnitudeGreaterZero' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!MagnitudeGreaterZero())
				yield return new ValidationResult() { Item = this, IssueSource = "MagnitudeGreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
