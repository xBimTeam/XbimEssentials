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
	public partial class IfcCartesianTransformationOperator3DnonUniform : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator3DnonUniform");

		/// <summary>
		/// Tests the express where clause Scale2GreaterZero
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool Scale2GreaterZero() {
			var retVal = false;
			try {
				retVal = Scl2 > 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'Scale2GreaterZero' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause Scale3GreaterZero
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool Scale3GreaterZero() {
			var retVal = false;
			try {
				retVal = Scl3 > 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'Scale3GreaterZero' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!Scale2GreaterZero())
				yield return new ValidationResult() { Item = this, IssueSource = "Scale2GreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
			if (!Scale3GreaterZero())
				yield return new ValidationResult() { Item = this, IssueSource = "Scale3GreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
