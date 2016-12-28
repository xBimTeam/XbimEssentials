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
	public partial class IfcPropertySetTemplate : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcPropertySetTemplate");

		/// <summary>
		/// Tests the express where clause ExistsName
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ExistsName() {
			var retVal = false;
			try {
				retVal = EXISTS(this/* as IfcRoot*/.Name);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ExistsName' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause UniquePropertyNames
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool UniquePropertyNames() {
			var retVal = false;
			try {
				retVal = IfcUniquePropertyTemplateNames(HasPropertyTemplates);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'UniquePropertyNames' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ExistsName())
				yield return new ValidationResult() { Item = this, IssueSource = "ExistsName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!UniquePropertyNames())
				yield return new ValidationResult() { Item = this, IssueSource = "UniquePropertyNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
