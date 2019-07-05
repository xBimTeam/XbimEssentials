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
namespace Xbim.Ifc4.PropertyResource
{
	public partial class IfcComplexProperty : IExpressValidatable
	{
		public enum IfcComplexPropertyClause
		{
			WR21,
			WR22,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcComplexPropertyClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcComplexPropertyClause.WR21:
						retVal = Functions.SIZEOF(HasProperties.Where(temp => Object.ReferenceEquals(this, temp))) == 0;
						break;
					case IfcComplexPropertyClause.WR22:
						retVal = Functions.IfcUniquePropertyName(HasProperties);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PropertyResource.IfcComplexProperty>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcComplexProperty.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcComplexPropertyClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcComplexProperty.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcComplexPropertyClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcComplexProperty.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
