using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.QuantityResource
{
	public partial class IfcQuantityArea : IExpressValidatable
	{
		public enum IfcQuantityAreaClause
		{
			WR21,
			WR22,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcQuantityAreaClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcQuantityAreaClause.WR21:
						retVal = !(Functions.EXISTS(this/* as IfcPhysicalSimpleQuantity*/.Unit)) || (this/* as IfcPhysicalSimpleQuantity*/.Unit.UnitType == IfcUnitEnum.AREAUNIT);
						break;
					case IfcQuantityAreaClause.WR22:
						retVal = AreaValue >= 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.QuantityResource.IfcQuantityArea>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcQuantityArea.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcQuantityAreaClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcQuantityArea.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcQuantityAreaClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcQuantityArea.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
