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
namespace Xbim.Ifc4.UtilityResource
{
	public partial class IfcOwnerHistory : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.UtilityResource.IfcOwnerHistory");

		/// <summary>
		/// Tests the express where clause CorrectChangeAction
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectChangeAction() {
			var retVal = false;
			try {
				retVal = (EXISTS(LastModifiedDate)) || (!(EXISTS(LastModifiedDate)) && !(EXISTS(ChangeAction))) || (!(EXISTS(LastModifiedDate)) && EXISTS(ChangeAction) && ((ChangeAction == IfcChangeActionEnum.NOTDEFINED) || (ChangeAction == IfcChangeActionEnum.NOCHANGE)));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectChangeAction' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!CorrectChangeAction())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectChangeAction", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
