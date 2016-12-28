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
	public partial class IfcRevolvedAreaSolid : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcRevolvedAreaSolid");

		/// <summary>
		/// Tests the express where clause AxisStartInXY
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool AxisStartInXY() {
			var retVal = false;
			try {
				retVal = Axis.Location.Coordinates.ToArray()[2] == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'AxisStartInXY' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause AxisDirectionInXY
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool AxisDirectionInXY() {
			var retVal = false;
			try {
				retVal = Axis.Z.DirectionRatios().ToArray()[2] == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'AxisDirectionInXY' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!AxisStartInXY())
				yield return new ValidationResult() { Item = this, IssueSource = "AxisStartInXY", IssueType = ValidationFlags.EntityWhereClauses };
			if (!AxisDirectionInXY())
				yield return new ValidationResult() { Item = this, IssueSource = "AxisDirectionInXY", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
