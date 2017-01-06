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
namespace Xbim.Ifc2x3.QuantityResource
{
	public partial class IfcQuantityTime : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.QuantityResource.IfcQuantityTime");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcQuantityTime clause) {
			var retVal = false;
			if (clause == Where.IfcQuantityTime.WR21) {
				try {
					retVal = !(EXISTS(this/* as IfcPhysicalSimpleQuantity*/.Unit)) || (this/* as IfcPhysicalSimpleQuantity*/.Unit.UnitType == IfcUnitEnum.TIMEUNIT);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcQuantityTime.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcQuantityTime.WR22) {
				try {
					retVal = TimeValue >= 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcQuantityTime.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcQuantityTime.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcQuantityTime.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcQuantityTime.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcQuantityTime.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcQuantityTime
	{
		public static readonly IfcQuantityTime WR21 = new IfcQuantityTime();
		public static readonly IfcQuantityTime WR22 = new IfcQuantityTime();
		protected IfcQuantityTime() {}
	}
}
