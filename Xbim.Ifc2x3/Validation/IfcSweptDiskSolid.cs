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
	public partial class IfcSweptDiskSolid : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcSweptDiskSolid");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSweptDiskSolid clause) {
			var retVal = false;
			if (clause == Where.IfcSweptDiskSolid.WR1) {
				try {
					retVal = Directrix.Dim == 3;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSweptDiskSolid.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSweptDiskSolid.WR2) {
				try {
					retVal = (!EXISTS(InnerRadius)) || (Radius > InnerRadius);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSweptDiskSolid.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSweptDiskSolid.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolid.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSweptDiskSolid.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolid.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcSweptDiskSolid
	{
		public static readonly IfcSweptDiskSolid WR1 = new IfcSweptDiskSolid();
		public static readonly IfcSweptDiskSolid WR2 = new IfcSweptDiskSolid();
		protected IfcSweptDiskSolid() {}
	}
}
