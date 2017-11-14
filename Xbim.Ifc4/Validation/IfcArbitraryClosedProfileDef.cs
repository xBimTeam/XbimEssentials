using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcArbitraryClosedProfileDef : IExpressValidatable
	{
		public enum IfcArbitraryClosedProfileDefClause
		{
			WR1,
			WR2,
			WR3,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcArbitraryClosedProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcArbitraryClosedProfileDefClause.WR1:
						retVal = OuterCurve.Dim == 2;
						break;
					case IfcArbitraryClosedProfileDefClause.WR2:
						retVal = !(Functions.TYPEOF(OuterCurve).Contains("IFC4.IFCLINE"));
						break;
					case IfcArbitraryClosedProfileDefClause.WR3:
						retVal = !(Functions.TYPEOF(OuterCurve).Contains("IFC4.IFCOFFSETCURVE2D"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ProfileResource.IfcArbitraryClosedProfileDef");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcArbitraryClosedProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcArbitraryClosedProfileDefClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryClosedProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcArbitraryClosedProfileDefClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryClosedProfileDef.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcArbitraryClosedProfileDefClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryClosedProfileDef.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
