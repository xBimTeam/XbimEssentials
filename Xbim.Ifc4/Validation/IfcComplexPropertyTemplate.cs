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
	public partial class IfcComplexPropertyTemplate : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcComplexPropertyTemplate");

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

		/// <summary>
		/// Tests the express where clause NoSelfReference
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoSelfReference() {
			var retVal = false;
			try {
				retVal = SIZEOF(HasPropertyTemplates.Where(temp => Object.ReferenceEquals(this, temp))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoSelfReference' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!UniquePropertyNames())
				yield return new ValidationResult() { Item = this, IssueSource = "UniquePropertyNames", IssueType = ValidationFlags.EntityWhereClauses };
			if (!NoSelfReference())
				yield return new ValidationResult() { Item = this, IssueSource = "NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
