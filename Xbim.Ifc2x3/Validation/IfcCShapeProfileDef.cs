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
	public partial class IfcCShapeProfileDef : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcCShapeProfileDef.WR1) {
				try {
					retVal = Girth < (Depth / 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcCShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCShapeProfileDef.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCShapeProfileDef.WR2) {
				try {
					retVal = !(EXISTS(InternalFilletRadius)) || ((InternalFilletRadius <= Width / 2) && (InternalFilletRadius <= Depth / 2));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcCShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCShapeProfileDef.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCShapeProfileDef.WR3) {
				try {
					retVal = (WallThickness < Width / 2) && (WallThickness < Depth / 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcCShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCShapeProfileDef.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCShapeProfileDef.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCShapeProfileDef.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCShapeProfileDef.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCShapeProfileDef.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcCShapeProfileDef
	{
		public static readonly IfcCShapeProfileDef WR1 = new IfcCShapeProfileDef();
		public static readonly IfcCShapeProfileDef WR2 = new IfcCShapeProfileDef();
		public static readonly IfcCShapeProfileDef WR3 = new IfcCShapeProfileDef();
		protected IfcCShapeProfileDef() {}
	}
}
