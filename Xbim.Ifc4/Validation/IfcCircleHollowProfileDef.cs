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
	public partial class IfcCircleHollowProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcCircleHollowProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCircleHollowProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcCircleHollowProfileDef.WR1) {
				try {
					retVal = WallThickness < this/* as IfcCircleProfileDef*/.Radius;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCircleHollowProfileDef.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCircleHollowProfileDef.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCircleHollowProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCircleHollowProfileDef
	{
		public static readonly IfcCircleHollowProfileDef WR1 = new IfcCircleHollowProfileDef();
		protected IfcCircleHollowProfileDef() {}
	}
}
