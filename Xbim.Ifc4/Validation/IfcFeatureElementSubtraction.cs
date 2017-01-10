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
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcFeatureElementSubtraction : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcFeatureElementSubtraction clause) {
			var retVal = false;
			if (clause == Where.IfcFeatureElementSubtraction.HasNoSubtraction) {
				try {
					retVal = SIZEOF(this/* as IfcElement*/.HasOpenings) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcFeatureElementSubtraction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcFeatureElementSubtraction.HasNoSubtraction' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcFeatureElementSubtraction.IsNotFilling) {
				try {
					retVal = SIZEOF(this/* as IfcElement*/.FillsVoids) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcFeatureElementSubtraction");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcFeatureElementSubtraction.IsNotFilling' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcFeatureElementSubtraction.HasNoSubtraction))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFeatureElementSubtraction.HasNoSubtraction", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcFeatureElementSubtraction.IsNotFilling))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFeatureElementSubtraction.IsNotFilling", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcFeatureElementSubtraction : IfcProduct
	{
		public static readonly IfcFeatureElementSubtraction HasNoSubtraction = new IfcFeatureElementSubtraction();
		public static readonly IfcFeatureElementSubtraction IsNotFilling = new IfcFeatureElementSubtraction();
		protected IfcFeatureElementSubtraction() {}
	}
}
