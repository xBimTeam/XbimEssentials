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
namespace Xbim.Ifc4.Kernel
{
	public partial class IfcObject : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcObject");

		/// <summary>
		/// Tests the express where clause UniquePropertySetNames
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool UniquePropertySetNames() {
			var retVal = false;
			try {
				retVal = ((SIZEOF(IsDefinedBy) == 0) || IfcUniqueDefinitionNames(IsDefinedBy));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'UniquePropertySetNames' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!UniquePropertySetNames())
				yield return new ValidationResult() { Item = this, IssueSource = "UniquePropertySetNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
