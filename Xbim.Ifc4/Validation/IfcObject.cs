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
	public partial class IfcObject : IExpressValidatable
	{
		public enum IfcObjectClause
		{
			UniquePropertySetNames,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcObjectClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcObjectClause.UniquePropertySetNames:
						retVal = ((Functions.SIZEOF(IsDefinedBy) == 0) || Functions.IfcUniqueDefinitionNames(IsDefinedBy));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcObject>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcObject.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcObjectClause.UniquePropertySetNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcObject.UniquePropertySetNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
