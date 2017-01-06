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
	public partial class IfcUShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcUShapeProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcUShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcUShapeProfileDef.WR21) {
				try {
					retVal = FlangeThickness < (Depth / 2);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcUShapeProfileDef.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcUShapeProfileDef.WR22) {
				try {
					retVal = WebThickness < FlangeWidth;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcUShapeProfileDef.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcUShapeProfileDef.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcUShapeProfileDef.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcUShapeProfileDef.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcUShapeProfileDef.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcUShapeProfileDef
	{
		public static readonly IfcUShapeProfileDef WR21 = new IfcUShapeProfileDef();
		public static readonly IfcUShapeProfileDef WR22 = new IfcUShapeProfileDef();
		protected IfcUShapeProfileDef() {}
	}
}
