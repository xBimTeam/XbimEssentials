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
	public partial class IfcTelecomAddress : IExpressValidatable
	{
		public enum IfcTelecomAddressClause
		{
			MinimumDataProvided,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTelecomAddressClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTelecomAddressClause.MinimumDataProvided:
						retVal = Functions.EXISTS(TelephoneNumbers) || Functions.EXISTS(FacsimileNumbers) || Functions.EXISTS(PagerNumber) || Functions.EXISTS(ElectronicMailAddresses) || Functions.EXISTS(WWWHomePageURL) || Functions.EXISTS(MessagingIDs);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ActorResource.IfcTelecomAddress>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTelecomAddress.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcTelecomAddressClause.MinimumDataProvided))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTelecomAddress.MinimumDataProvided", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
