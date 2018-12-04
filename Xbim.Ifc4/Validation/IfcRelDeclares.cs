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
	public partial class IfcRelDeclares : IExpressValidatable
	{
		public enum IfcRelDeclaresClause
		{
			NoSelfReference,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelDeclaresClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelDeclaresClause.NoSelfReference:
						retVal = Functions.SIZEOF(RelatedDefinitions.Where(Temp => Object.ReferenceEquals(RelatingContext, Temp))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcRelDeclares>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelDeclares.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRelDeclaresClause.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelDeclares.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
