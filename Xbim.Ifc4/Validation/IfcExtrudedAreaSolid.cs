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
	public partial class IfcExtrudedAreaSolid : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcExtrudedAreaSolid");

		/// <summary>
		/// Tests the express where clause ValidExtrusionDirection
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidExtrusionDirection() {
			var retVal = false;
			try {
				//retVal = IfcDotProduct(IfcDirection(NewArray(0, 0, 1)), this.ExtrudedDirection) != 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidExtrusionDirection' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidExtrusionDirection())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidExtrusionDirection", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
