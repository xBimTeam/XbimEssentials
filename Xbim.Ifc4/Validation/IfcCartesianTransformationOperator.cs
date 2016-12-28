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
	public partial class IfcCartesianTransformationOperator : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator");

		/// <summary>
		/// Tests the express where clause ScaleGreaterZero
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ScaleGreaterZero() {
			var retVal = false;
			try {
				retVal = Scl > 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ScaleGreaterZero' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ScaleGreaterZero())
				yield return new ValidationResult() { Item = this, IssueSource = "ScaleGreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
