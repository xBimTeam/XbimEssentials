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
	public partial class IfcComplexPropertyTemplate : IExpressValidatable
	{
		public enum IfcComplexPropertyTemplateClause
		{
			UniquePropertyNames,
			NoSelfReference,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcComplexPropertyTemplateClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcComplexPropertyTemplateClause.UniquePropertyNames:
						retVal = Functions.IfcUniquePropertyTemplateNames(HasPropertyTemplates);
						break;
					case IfcComplexPropertyTemplateClause.NoSelfReference:
						retVal = Functions.SIZEOF(HasPropertyTemplates.Where(temp => Object.ReferenceEquals(this, temp))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcComplexPropertyTemplate>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcComplexPropertyTemplate.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcComplexPropertyTemplateClause.UniquePropertyNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcComplexPropertyTemplate.UniquePropertyNames", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcComplexPropertyTemplateClause.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcComplexPropertyTemplate.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
