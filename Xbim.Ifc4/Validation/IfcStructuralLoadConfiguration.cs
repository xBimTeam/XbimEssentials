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
namespace Xbim.Ifc4.StructuralLoadResource
{
	public partial class IfcStructuralLoadConfiguration : IExpressValidatable
	{
		public enum IfcStructuralLoadConfigurationClause
		{
			ValidListSize,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralLoadConfigurationClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralLoadConfigurationClause.ValidListSize:
						retVal = !Functions.EXISTS(Locations) || (Functions.SIZEOF(Locations) == Functions.SIZEOF(Values));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.StructuralLoadResource.IfcStructuralLoadConfiguration>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStructuralLoadConfiguration.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcStructuralLoadConfigurationClause.ValidListSize))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralLoadConfiguration.ValidListSize", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
