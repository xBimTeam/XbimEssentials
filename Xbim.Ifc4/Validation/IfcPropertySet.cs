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
	public partial class IfcPropertySet : IExpressValidatable
	{
		public enum IfcPropertySetClause
		{
			ExistsName,
			UniquePropertyNames,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPropertySetClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPropertySetClause.ExistsName:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcPropertySetClause.UniquePropertyNames:
						retVal = Functions.IfcUniquePropertyName(HasProperties);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcPropertySet>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPropertySet.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPropertySetClause.ExistsName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySet.ExistsName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPropertySetClause.UniquePropertyNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySet.UniquePropertyNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
