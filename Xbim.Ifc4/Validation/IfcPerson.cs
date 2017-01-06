using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ActorResource
{
	public partial class IfcPerson : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ActorResource.IfcPerson");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPerson clause) {
			var retVal = false;
			if (clause == Where.IfcPerson.IdentifiablePerson) {
				try {
					retVal = EXISTS(Identification) || EXISTS(FamilyName) || EXISTS(GivenName);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPerson.IdentifiablePerson' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPerson.ValidSetOfNames) {
				try {
					retVal = !EXISTS(MiddleNames) || EXISTS(FamilyName) || EXISTS(GivenName);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPerson.ValidSetOfNames' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPerson.IdentifiablePerson))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPerson.IdentifiablePerson", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPerson.ValidSetOfNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPerson.ValidSetOfNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPerson
	{
		public static readonly IfcPerson IdentifiablePerson = new IfcPerson();
		public static readonly IfcPerson ValidSetOfNames = new IfcPerson();
		protected IfcPerson() {}
	}
}
