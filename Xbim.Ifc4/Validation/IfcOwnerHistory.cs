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
namespace Xbim.Ifc4.UtilityResource
{
	public partial class IfcOwnerHistory : IExpressValidatable
	{
		public enum IfcOwnerHistoryClause
		{
			CorrectChangeAction,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcOwnerHistoryClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcOwnerHistoryClause.CorrectChangeAction:
						retVal = (Functions.EXISTS(LastModifiedDate)) || (!(Functions.EXISTS(LastModifiedDate)) && !(Functions.EXISTS(ChangeAction))) || (!(Functions.EXISTS(LastModifiedDate)) && Functions.EXISTS(ChangeAction) && ((ChangeAction == IfcChangeActionEnum.NOTDEFINED) || (ChangeAction == IfcChangeActionEnum.NOCHANGE)));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.UtilityResource.IfcOwnerHistory>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcOwnerHistory.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcOwnerHistoryClause.CorrectChangeAction))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcOwnerHistory.CorrectChangeAction", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
