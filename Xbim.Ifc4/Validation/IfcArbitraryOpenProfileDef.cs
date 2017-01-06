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
	public partial class IfcArbitraryOpenProfileDef : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcArbitraryOpenProfileDef");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcArbitraryOpenProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcArbitraryOpenProfileDef.WR11) {
				try {
					retVal = (TYPEOF(this).Contains("IFC4.IFCCENTERLINEPROFILEDEF")) || (this/* as IfcProfileDef*/.ProfileType == IfcProfileTypeEnum.CURVE);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcArbitraryOpenProfileDef.WR11' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcArbitraryOpenProfileDef.WR12) {
				try {
					retVal = Curve.Dim == 2;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcArbitraryOpenProfileDef.WR12' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcArbitraryOpenProfileDef.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryOpenProfileDef.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcArbitraryOpenProfileDef.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryOpenProfileDef.WR12", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcArbitraryOpenProfileDef
	{
		public static readonly IfcArbitraryOpenProfileDef WR11 = new IfcArbitraryOpenProfileDef();
		public static readonly IfcArbitraryOpenProfileDef WR12 = new IfcArbitraryOpenProfileDef();
		protected IfcArbitraryOpenProfileDef() {}
	}
}
