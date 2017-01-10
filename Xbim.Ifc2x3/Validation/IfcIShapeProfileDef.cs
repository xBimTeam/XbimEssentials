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
	public partial class IfcIShapeProfileDef : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcIShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcIShapeProfileDef.WR1) {
				try {
					retVal = FlangeThickness < (OverallDepth / 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcIShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcIShapeProfileDef.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcIShapeProfileDef.WR2) {
				try {
					retVal = WebThickness < OverallWidth;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcIShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcIShapeProfileDef.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcIShapeProfileDef.WR3) {
				try {
					retVal = !(EXISTS(FilletRadius)) || ((FilletRadius <= (OverallWidth - WebThickness) / 2) && (FilletRadius <= (OverallDepth - (2 * FlangeThickness)) / 2));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcIShapeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcIShapeProfileDef.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcIShapeProfileDef.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcIShapeProfileDef.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcIShapeProfileDef.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcIShapeProfileDef
	{
		public static readonly IfcIShapeProfileDef WR1 = new IfcIShapeProfileDef();
		public static readonly IfcIShapeProfileDef WR2 = new IfcIShapeProfileDef();
		public static readonly IfcIShapeProfileDef WR3 = new IfcIShapeProfileDef();
		protected IfcIShapeProfileDef() {}
	}
}
