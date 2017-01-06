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
namespace Xbim.Ifc2x3.ProfileResource
{
	public partial class IfcArbitraryProfileDefWithVoids : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcArbitraryProfileDefWithVoids");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcArbitraryProfileDefWithVoids clause) {
			var retVal = false;
			if (clause == Where.IfcArbitraryProfileDefWithVoids.WR1) {
				try {
					retVal = this/* as IfcProfileDef*/.ProfileType == IfcProfileTypeEnum.AREA;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcArbitraryProfileDefWithVoids.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcArbitraryProfileDefWithVoids.WR2) {
				try {
					retVal = SIZEOF(InnerCurves.Where(temp => temp.Dim != 2)) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcArbitraryProfileDefWithVoids.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcArbitraryProfileDefWithVoids.WR3) {
				try {
					retVal = SIZEOF(InnerCurves.Where(temp => TYPEOF(temp).Contains("IFC2X3.IFCLINE"))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcArbitraryProfileDefWithVoids.WR3' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcArbitraryClosedProfileDef)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcArbitraryProfileDefWithVoids.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryProfileDefWithVoids.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcArbitraryProfileDefWithVoids.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryProfileDefWithVoids.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcArbitraryProfileDefWithVoids.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryProfileDefWithVoids.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcArbitraryProfileDefWithVoids : IfcArbitraryClosedProfileDef
	{
		public new static readonly IfcArbitraryProfileDefWithVoids WR1 = new IfcArbitraryProfileDefWithVoids();
		public new static readonly IfcArbitraryProfileDefWithVoids WR2 = new IfcArbitraryProfileDefWithVoids();
		public new static readonly IfcArbitraryProfileDefWithVoids WR3 = new IfcArbitraryProfileDefWithVoids();
		protected IfcArbitraryProfileDefWithVoids() {}
	}
}
