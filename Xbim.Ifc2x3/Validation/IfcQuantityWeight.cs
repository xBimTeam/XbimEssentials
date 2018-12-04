using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.QuantityResource
{
	public partial class IfcQuantityWeight : IExpressValidatable
	{
		public enum IfcQuantityWeightClause
		{
			WR21,
			WR22,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcQuantityWeightClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcQuantityWeightClause.WR21:
						retVal = !(Functions.EXISTS(this/* as IfcPhysicalSimpleQuantity*/.Unit)) || (this/* as IfcPhysicalSimpleQuantity*/.Unit.UnitType == IfcUnitEnum.MASSUNIT);
						break;
					case IfcQuantityWeightClause.WR22:
						retVal = WeightValue >= 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.QuantityResource.IfcQuantityWeight>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcQuantityWeight.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcQuantityWeightClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcQuantityWeight.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcQuantityWeightClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcQuantityWeight.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
