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
namespace Xbim.Ifc2x3.ElectricalDomain
{
	public partial class IfcElectricDistributionPoint : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ElectricalDomain.IfcElectricDistributionPoint");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcElectricDistributionPoint clause) {
			var retVal = false;
			if (clause == Where.IfcElectricDistributionPoint.WR31) {
				try {
					retVal = (DistributionPointFunction != IfcElectricDistributionPointFunctionEnum.USERDEFINED) || ((DistributionPointFunction == IfcElectricDistributionPointFunctionEnum.USERDEFINED) && EXISTS(this/* as IfcElectricDistributionPoint*/.UserDefinedFunction));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcElectricDistributionPoint.WR31' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcElectricDistributionPoint.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcElectricDistributionPoint.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcElectricDistributionPoint : IfcProduct
	{
		public static readonly IfcElectricDistributionPoint WR31 = new IfcElectricDistributionPoint();
		protected IfcElectricDistributionPoint() {}
	}
}
