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
namespace Xbim.Ifc4.ArchitectureDomain
{
	public partial class IfcWindowPanelProperties : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ArchitectureDomain.IfcWindowPanelProperties");

		/// <summary>
		/// Tests the express where clause ApplicableToType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ApplicableToType() {
			var retVal = false;
			try {
				retVal = (EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0])) && ((TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0]).Contains("IFC4.IFCWINDOWTYPE")) || (TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0]).Contains("IFC4.IFCWINDOWSTYLE")));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ApplicableToType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ApplicableToType())
				yield return new ValidationResult() { Item = this, IssueSource = "ApplicableToType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
