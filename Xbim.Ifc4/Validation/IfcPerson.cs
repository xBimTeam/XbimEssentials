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
namespace Xbim.Ifc4.ActorResource
{
	public partial class IfcPerson : IExpressValidatable
	{
		public enum IfcPersonClause
		{
			IdentifiablePerson,
			ValidSetOfNames,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPersonClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPersonClause.IdentifiablePerson:
						retVal = Functions.EXISTS(Identification) || Functions.EXISTS(FamilyName) || Functions.EXISTS(GivenName);
						break;
					case IfcPersonClause.ValidSetOfNames:
						retVal = !Functions.EXISTS(MiddleNames) || Functions.EXISTS(FamilyName) || Functions.EXISTS(GivenName);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ActorResource.IfcPerson>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPerson.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPersonClause.IdentifiablePerson))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPerson.IdentifiablePerson", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPersonClause.ValidSetOfNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPerson.ValidSetOfNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
