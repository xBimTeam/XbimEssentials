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
	public partial class IfcTShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcTShapeProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcTShapeProfileDef.WR1) {
				try {
					retVal = FlangeThickness < Depth;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTShapeProfileDef.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTShapeProfileDef.WR2) {
				try {
					retVal = WebThickness < FlangeWidth;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTShapeProfileDef.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTShapeProfileDef.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTShapeProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTShapeProfileDef.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTShapeProfileDef.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcTShapeProfileDef
	{
		public static readonly IfcTShapeProfileDef WR1 = new IfcTShapeProfileDef();
		public static readonly IfcTShapeProfileDef WR2 = new IfcTShapeProfileDef();
		protected IfcTShapeProfileDef() {}
	}
}
