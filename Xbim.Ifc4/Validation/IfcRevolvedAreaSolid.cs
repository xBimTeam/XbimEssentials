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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRevolvedAreaSolid clause) {
			var retVal = false;
			if (clause == Where.IfcRevolvedAreaSolid.AxisStartInXY) {
				try {
					retVal = Axis.Location.Coordinates.ToArray()[2] == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRevolvedAreaSolid.AxisStartInXY' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRevolvedAreaSolid.AxisDirectionInXY) {
				try {
					retVal = Axis.Z.DirectionRatios().ToArray()[2] == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRevolvedAreaSolid.AxisDirectionInXY' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcSweptAreaSolid)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcRevolvedAreaSolid.AxisStartInXY))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRevolvedAreaSolid.AxisStartInXY", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRevolvedAreaSolid.AxisDirectionInXY))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRevolvedAreaSolid.AxisDirectionInXY", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRevolvedAreaSolid : IfcSweptAreaSolid
	{
		public static readonly IfcRevolvedAreaSolid AxisStartInXY = new IfcRevolvedAreaSolid();
		public static readonly IfcRevolvedAreaSolid AxisDirectionInXY = new IfcRevolvedAreaSolid();
		protected IfcRevolvedAreaSolid() {}
	}
}
