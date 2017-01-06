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
	public partial class IfcLShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcLShapeProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcLShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcLShapeProfileDef.WR21) {
				try {
					retVal = Thickness < Depth;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcLShapeProfileDef.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcLShapeProfileDef.WR22) {
				try {
					retVal = !(EXISTS(Width)) || (Thickness < Width);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcLShapeProfileDef.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcLShapeProfileDef.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcLShapeProfileDef.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcLShapeProfileDef.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcLShapeProfileDef.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcLShapeProfileDef
	{
		public static readonly IfcLShapeProfileDef WR21 = new IfcLShapeProfileDef();
		public static readonly IfcLShapeProfileDef WR22 = new IfcLShapeProfileDef();
		protected IfcLShapeProfileDef() {}
	}
}
