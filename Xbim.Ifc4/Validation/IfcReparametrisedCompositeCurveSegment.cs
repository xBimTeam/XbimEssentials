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
	public partial class IfcReparametrisedCompositeCurveSegment : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcReparametrisedCompositeCurveSegment");

		/// <summary>
		/// Tests the express where clause PositiveLengthParameter
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool PositiveLengthParameter() {
			var retVal = false;
			try {
				retVal = ParamLength > 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'PositiveLengthParameter' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!PositiveLengthParameter())
				yield return new ValidationResult() { Item = this, IssueSource = "PositiveLengthParameter", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
