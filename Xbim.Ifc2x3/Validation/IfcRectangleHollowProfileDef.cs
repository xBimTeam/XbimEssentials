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
	public partial class IfcRectangleHollowProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcRectangleHollowProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRectangleHollowProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcRectangleHollowProfileDef.WR31) {
				try {
					retVal = (WallThickness < (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (WallThickness < (this/* as IfcRectangleProfileDef*/.YDim / 2));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangleHollowProfileDef.WR31' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangleHollowProfileDef.WR32) {
				try {
					retVal = !(EXISTS(OuterFilletRadius)) || ((OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2)));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangleHollowProfileDef.WR32' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangleHollowProfileDef.WR33) {
				try {
					retVal = !(EXISTS(InnerFilletRadius)) || ((InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2 - WallThickness)) && (InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2 - WallThickness)));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangleHollowProfileDef.WR33' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRectangleHollowProfileDef.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangleHollowProfileDef.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangleHollowProfileDef.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.WR33", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcRectangleHollowProfileDef
	{
		public static readonly IfcRectangleHollowProfileDef WR31 = new IfcRectangleHollowProfileDef();
		public static readonly IfcRectangleHollowProfileDef WR32 = new IfcRectangleHollowProfileDef();
		public static readonly IfcRectangleHollowProfileDef WR33 = new IfcRectangleHollowProfileDef();
		protected IfcRectangleHollowProfileDef() {}
	}
}
