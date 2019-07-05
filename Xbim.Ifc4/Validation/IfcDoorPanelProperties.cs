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
namespace Xbim.Ifc4.ArchitectureDomain
{
	public partial class IfcDoorPanelProperties : IExpressValidatable
	{
		public enum IfcDoorPanelPropertiesClause
		{
			ApplicableToType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDoorPanelPropertiesClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDoorPanelPropertiesClause.ApplicableToType:
						retVal = (Functions.EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0))) && ((Functions.TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC4.IFCDOORTYPE")) || (Functions.TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC4.IFCDOORSTYLE")));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ArchitectureDomain.IfcDoorPanelProperties>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcDoorPanelProperties.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDoorPanelPropertiesClause.ApplicableToType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDoorPanelProperties.ApplicableToType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
