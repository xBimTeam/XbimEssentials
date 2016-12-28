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
		/// Tests the express where clause UniqueQuantityNames
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool UniqueQuantityNames() {
			var retVal = false;
			try {
				retVal = IfcUniqueQuantityNames(Quantities);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'UniqueQuantityNames' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!UniqueQuantityNames())
				yield return new ValidationResult() { Item = this, IssueSource = "UniqueQuantityNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
