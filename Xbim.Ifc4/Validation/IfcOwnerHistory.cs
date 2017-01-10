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
						retVal = (EXISTS(LastModifiedDate)) || (!(EXISTS(LastModifiedDate)) && !(EXISTS(ChangeAction))) || (!(EXISTS(LastModifiedDate)) && EXISTS(ChangeAction) && ((ChangeAction == IfcChangeActionEnum.NOTDEFINED) || (ChangeAction == IfcChangeActionEnum.NOCHANGE)));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.UtilityResource.IfcOwnerHistory");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcOwnerHistory.{0}' for #{1}.", clause,EntityLabel), ex);
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
