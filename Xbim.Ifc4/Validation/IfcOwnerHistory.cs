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

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcOwnerHistory clause) {
			var retVal = false;
			if (clause == Where.IfcOwnerHistory.CorrectChangeAction) {
				try {
					retVal = (EXISTS(LastModifiedDate)) || (!(EXISTS(LastModifiedDate)) && !(EXISTS(ChangeAction))) || (!(EXISTS(LastModifiedDate)) && EXISTS(ChangeAction) && ((ChangeAction == IfcChangeActionEnum.NOTDEFINED) || (ChangeAction == IfcChangeActionEnum.NOCHANGE)));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.UtilityResource.IfcOwnerHistory");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcOwnerHistory.CorrectChangeAction' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcOwnerHistory.CorrectChangeAction))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcOwnerHistory.CorrectChangeAction", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcOwnerHistory
	{
		public static readonly IfcOwnerHistory CorrectChangeAction = new IfcOwnerHistory();
		protected IfcOwnerHistory() {}
	}
}
