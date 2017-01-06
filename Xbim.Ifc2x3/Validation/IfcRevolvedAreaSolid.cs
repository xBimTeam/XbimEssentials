using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.GeometricModelResource
{
	public partial class IfcRevolvedAreaSolid : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcRevolvedAreaSolid");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRevolvedAreaSolid clause) {
			var retVal = false;
			if (clause == Where.IfcRevolvedAreaSolid.WR31) {
				try {
					retVal = Axis.Location.Coordinates.ToArray()[2] == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRevolvedAreaSolid.WR31' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRevolvedAreaSolid.WR32) {
				try {
					retVal = Axis.Z.DirectionRatios().ToArray()[2] == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRevolvedAreaSolid.WR32' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcRevolvedAreaSolid.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRevolvedAreaSolid.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRevolvedAreaSolid.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRevolvedAreaSolid.WR32", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcRevolvedAreaSolid : IfcSweptAreaSolid
	{
		public static readonly IfcRevolvedAreaSolid WR31 = new IfcRevolvedAreaSolid();
		public static readonly IfcRevolvedAreaSolid WR32 = new IfcRevolvedAreaSolid();
		protected IfcRevolvedAreaSolid() {}
	}
}
