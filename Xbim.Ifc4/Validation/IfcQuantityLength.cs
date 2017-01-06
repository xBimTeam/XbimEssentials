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
namespace Xbim.Ifc4.QuantityResource
{
	public partial class IfcQuantityLength : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.QuantityResource.IfcQuantityLength");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcQuantityLength clause) {
			var retVal = false;
			if (clause == Where.IfcQuantityLength.WR21) {
				try {
					retVal = !(EXISTS(this/* as IfcPhysicalSimpleQuantity*/.Unit)) || (this/* as IfcPhysicalSimpleQuantity*/.Unit.UnitType == IfcUnitEnum.LENGTHUNIT);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcQuantityLength.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcQuantityLength.WR22) {
				try {
					retVal = LengthValue >= 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcQuantityLength.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcQuantityLength.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcQuantityLength.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcQuantityLength.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcQuantityLength.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcQuantityLength
	{
		public static readonly IfcQuantityLength WR21 = new IfcQuantityLength();
		public static readonly IfcQuantityLength WR22 = new IfcQuantityLength();
		protected IfcQuantityLength() {}
	}
}
