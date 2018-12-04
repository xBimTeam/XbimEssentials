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
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcElementQuantity : IExpressValidatable
	{
		public enum IfcElementQuantityClause
		{
			UniqueQuantityNames,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcElementQuantityClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcElementQuantityClause.UniqueQuantityNames:
						retVal = Functions.IfcUniqueQuantityNames(Quantities);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProductExtension.IfcElementQuantity>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcElementQuantity.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcElementQuantityClause.UniqueQuantityNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcElementQuantity.UniqueQuantityNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
