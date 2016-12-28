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
namespace Xbim.Ifc4.ConstraintResource
{
	public partial class IfcConstraint : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ConstraintResource.IfcConstraint");

		/// <summary>
		/// Tests the express where clause WR11
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR11() {
			var retVal = false;
			try {
				retVal = (ConstraintGrade != IfcConstraintEnum.USERDEFINED) || ((ConstraintGrade == IfcConstraintEnum.USERDEFINED) && EXISTS(this/* as IfcConstraint*/.UserDefinedGrade));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR11' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR11())
				yield return new ValidationResult() { Item = this, IssueSource = "WR11", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
