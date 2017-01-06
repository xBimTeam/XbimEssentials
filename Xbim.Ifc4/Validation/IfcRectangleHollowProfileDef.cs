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
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcRectangleHollowProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcRectangleHollowProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRectangleHollowProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcRectangleHollowProfileDef.ValidWallThickness) {
				try {
					retVal = (WallThickness < (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (WallThickness < (this/* as IfcRectangleProfileDef*/.YDim / 2));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangleHollowProfileDef.ValidWallThickness' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangleHollowProfileDef.ValidInnerRadius) {
				try {
					retVal = !(EXISTS(InnerFilletRadius)) || ((InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2 - WallThickness)) && (InnerFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2 - WallThickness)));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangleHollowProfileDef.ValidInnerRadius' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRectangleHollowProfileDef.ValidOuterRadius) {
				try {
					retVal = !(EXISTS(OuterFilletRadius)) || ((OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (OuterFilletRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2)));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRectangleHollowProfileDef.ValidOuterRadius' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRectangleHollowProfileDef.ValidWallThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.ValidWallThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangleHollowProfileDef.ValidInnerRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.ValidInnerRadius", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRectangleHollowProfileDef.ValidOuterRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRectangleHollowProfileDef.ValidOuterRadius", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRectangleHollowProfileDef
	{
		public static readonly IfcRectangleHollowProfileDef ValidWallThickness = new IfcRectangleHollowProfileDef();
		public static readonly IfcRectangleHollowProfileDef ValidInnerRadius = new IfcRectangleHollowProfileDef();
		public static readonly IfcRectangleHollowProfileDef ValidOuterRadius = new IfcRectangleHollowProfileDef();
		protected IfcRectangleHollowProfileDef() {}
	}
}
