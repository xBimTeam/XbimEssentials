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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcWindowPanelProperties clause) {
			var retVal = false;
			if (clause == Where.IfcWindowPanelProperties.ApplicableToType) {
				try {
					retVal = (EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0])) && ((TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0]).Contains("IFC4.IFCWINDOWTYPE")) || (TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0]).Contains("IFC4.IFCWINDOWSTYLE")));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcWindowPanelProperties.ApplicableToType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcWindowPanelProperties.ApplicableToType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowPanelProperties.ApplicableToType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcWindowPanelProperties
	{
		public static readonly IfcWindowPanelProperties ApplicableToType = new IfcWindowPanelProperties();
		protected IfcWindowPanelProperties() {}
	}
}
