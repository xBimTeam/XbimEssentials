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
	public partial class IfcCShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcCShapeProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcCShapeProfileDef.ValidGirth) {
				try {
					retVal = Girth < (Depth / 2);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCShapeProfileDef.ValidGirth' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCShapeProfileDef.ValidInternalFilletRadius) {
				try {
					retVal = !(EXISTS(InternalFilletRadius)) || ((InternalFilletRadius <= Width / 2 - WallThickness) && (InternalFilletRadius <= Depth / 2 - WallThickness));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCShapeProfileDef.ValidInternalFilletRadius' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCShapeProfileDef.ValidWallThickness) {
				try {
					retVal = (WallThickness < Width / 2) && (WallThickness < Depth / 2);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCShapeProfileDef.ValidWallThickness' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCShapeProfileDef.ValidGirth))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.ValidGirth", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCShapeProfileDef.ValidInternalFilletRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.ValidInternalFilletRadius", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCShapeProfileDef.ValidWallThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.ValidWallThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCShapeProfileDef
	{
		public static readonly IfcCShapeProfileDef ValidGirth = new IfcCShapeProfileDef();
		public static readonly IfcCShapeProfileDef ValidInternalFilletRadius = new IfcCShapeProfileDef();
		public static readonly IfcCShapeProfileDef ValidWallThickness = new IfcCShapeProfileDef();
		protected IfcCShapeProfileDef() {}
	}
}
