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
	public partial class IfcElementQuantity : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcElementQuantity");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcElementQuantity clause) {
			var retVal = false;
			if (clause == Where.IfcElementQuantity.UniqueQuantityNames) {
				try {
					retVal = IfcUniqueQuantityNames(Quantities);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcElementQuantity.UniqueQuantityNames' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcElementQuantity.UniqueQuantityNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcElementQuantity.UniqueQuantityNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcElementQuantity
	{
		public static readonly IfcElementQuantity UniqueQuantityNames = new IfcElementQuantity();
		protected IfcElementQuantity() {}
	}
}
