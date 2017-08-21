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
	public partial class IfcTypeObject : IExpressValidatable
	{
		public enum IfcTypeObjectClause
		{
			NameRequired,
			UniquePropertySetNames,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTypeObjectClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTypeObjectClause.NameRequired:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcTypeObjectClause.UniquePropertySetNames:
						retVal = (!(Functions.EXISTS(HasPropertySets))) || Functions.IfcUniquePropertySetNames(HasPropertySets);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcTypeObject>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTypeObject.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTypeObjectClause.NameRequired))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTypeObject.NameRequired", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTypeObjectClause.UniquePropertySetNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTypeObject.UniquePropertySetNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
