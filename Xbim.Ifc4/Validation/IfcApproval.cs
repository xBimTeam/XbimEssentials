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
namespace Xbim.Ifc4.ApprovalResource
{
	public partial class IfcApproval : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ApprovalResource.IfcApproval");

		/// <summary>
		/// Tests the express where clause HasIdentifierOrName
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasIdentifierOrName() {
			var retVal = false;
			try {
				retVal = EXISTS(Identifier) || EXISTS(Name);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasIdentifierOrName' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!HasIdentifierOrName())
				yield return new ValidationResult() { Item = this, IssueSource = "HasIdentifierOrName", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
