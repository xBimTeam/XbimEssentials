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
	public partial class IfcTelecomAddress : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ActorResource.IfcTelecomAddress");

		/// <summary>
		/// Tests the express where clause MinimumDataProvided
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MinimumDataProvided() {
			var retVal = false;
			try {
				retVal = EXISTS(TelephoneNumbers) || EXISTS(FacsimileNumbers) || EXISTS(PagerNumber) || EXISTS(ElectronicMailAddresses) || EXISTS(WWWHomePageURL) || EXISTS(MessagingIDs);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MinimumDataProvided' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!MinimumDataProvided())
				yield return new ValidationResult() { Item = this, IssueSource = "MinimumDataProvided", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
