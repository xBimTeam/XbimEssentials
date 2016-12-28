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
		/// Tests the express where clause IdentifiablePerson
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool IdentifiablePerson() {
			var retVal = false;
			try {
				retVal = EXISTS(Identification) || EXISTS(FamilyName) || EXISTS(GivenName);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'IdentifiablePerson' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ValidSetOfNames
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidSetOfNames() {
			var retVal = false;
			try {
				retVal = !EXISTS(MiddleNames) || EXISTS(FamilyName) || EXISTS(GivenName);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ValidSetOfNames' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!IdentifiablePerson())
				yield return new ValidationResult() { Item = this, IssueSource = "IdentifiablePerson", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidSetOfNames())
				yield return new ValidationResult() { Item = this, IssueSource = "ValidSetOfNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
