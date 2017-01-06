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
	public partial class IfcUShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcUShapeProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcUShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcUShapeProfileDef.ValidFlangeThickness) {
				try {
					retVal = FlangeThickness < (Depth / 2);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcUShapeProfileDef.ValidFlangeThickness' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcUShapeProfileDef.ValidWebThickness) {
				try {
					retVal = WebThickness < FlangeWidth;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcUShapeProfileDef.ValidWebThickness' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcUShapeProfileDef.ValidFlangeThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcUShapeProfileDef.ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcUShapeProfileDef.ValidWebThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcUShapeProfileDef.ValidWebThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcUShapeProfileDef
	{
		public static readonly IfcUShapeProfileDef ValidFlangeThickness = new IfcUShapeProfileDef();
		public static readonly IfcUShapeProfileDef ValidWebThickness = new IfcUShapeProfileDef();
		protected IfcUShapeProfileDef() {}
	}
}
