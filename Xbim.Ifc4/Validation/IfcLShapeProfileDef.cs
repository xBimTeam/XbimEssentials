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
	public partial class IfcLShapeProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcLShapeProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcLShapeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcLShapeProfileDef.ValidThickness) {
				try {
					retVal = (Thickness < Depth) && (!(EXISTS(Width)) || (Thickness < Width));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcLShapeProfileDef.ValidThickness' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcLShapeProfileDef.ValidThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcLShapeProfileDef.ValidThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcLShapeProfileDef
	{
		public static readonly IfcLShapeProfileDef ValidThickness = new IfcLShapeProfileDef();
		protected IfcLShapeProfileDef() {}
	}
}
