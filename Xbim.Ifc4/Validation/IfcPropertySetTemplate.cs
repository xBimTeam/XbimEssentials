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
namespace Xbim.Ifc4.Kernel
{
	public partial class IfcPropertySetTemplate : IExpressValidatable
	{
		public enum IfcPropertySetTemplateClause
		{
			ExistsName,
			UniquePropertyNames,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPropertySetTemplateClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPropertySetTemplateClause.ExistsName:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcPropertySetTemplateClause.UniquePropertyNames:
						retVal = Functions.IfcUniquePropertyTemplateNames(HasPropertyTemplates);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcPropertySetTemplate>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPropertySetTemplate.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPropertySetTemplateClause.ExistsName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySetTemplate.ExistsName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPropertySetTemplateClause.UniquePropertyNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySetTemplate.UniquePropertyNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
