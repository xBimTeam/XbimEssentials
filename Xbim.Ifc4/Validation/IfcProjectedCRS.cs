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
	public partial class IfcProjectedCRS : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcProjectedCRS");

		/// <summary>
		/// Tests the express where clause IsLengthUnit
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool IsLengthUnit() {
			var retVal = false;
			try {
				retVal = !(EXISTS(MapUnit)) || (MapUnit.UnitType == IfcUnitEnum.LENGTHUNIT);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'IsLengthUnit' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!IsLengthUnit())
				yield return new ValidationResult() { Item = this, IssueSource = "IsLengthUnit", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
