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
	public partial class IfcTypeObject : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcTypeObject");

		/// <summary>
		/// Tests the express where clause NameRequired
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NameRequired() {
			var retVal = false;
			try {
				retVal = EXISTS(this/* as IfcRoot*/.Name);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NameRequired' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause UniquePropertySetNames
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool UniquePropertySetNames() {
			var retVal = false;
			try {
				retVal = (!(EXISTS(HasPropertySets))) || IfcUniquePropertySetNames(HasPropertySets);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'UniquePropertySetNames' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!NameRequired())
				yield return new ValidationResult() { Item = this, IssueSource = "NameRequired", IssueType = ValidationFlags.EntityWhereClauses };
			if (!UniquePropertySetNames())
				yield return new ValidationResult() { Item = this, IssueSource = "UniquePropertySetNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
