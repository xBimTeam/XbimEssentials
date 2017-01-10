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
		public enum IfcApprovalClause
		{
			HasIdentifierOrName,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcApprovalClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcApprovalClause.HasIdentifierOrName:
						retVal = EXISTS(Identifier) || EXISTS(Name);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ApprovalResource.IfcApproval");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcApproval.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcApprovalClause.HasIdentifierOrName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcApproval.HasIdentifierOrName", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
