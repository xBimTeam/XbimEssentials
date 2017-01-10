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
		public enum IfcWindowPanelPropertiesClause
		{
			ApplicableToType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcWindowPanelPropertiesClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcWindowPanelPropertiesClause.ApplicableToType:
						retVal = (EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0))) && ((TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC4.IFCWINDOWTYPE")) || (TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC4.IFCWINDOWSTYLE")));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ArchitectureDomain.IfcWindowPanelProperties");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcWindowPanelProperties.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcWindowPanelPropertiesClause.ApplicableToType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowPanelProperties.ApplicableToType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
