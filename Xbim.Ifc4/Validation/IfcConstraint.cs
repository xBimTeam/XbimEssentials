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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcConstraint clause) {
			var retVal = false;
			if (clause == Where.IfcConstraint.WR11) {
				try {
					retVal = (ConstraintGrade != IfcConstraintEnum.USERDEFINED) || ((ConstraintGrade == IfcConstraintEnum.USERDEFINED) && EXISTS(this/* as IfcConstraint*/.UserDefinedGrade));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcConstraint.WR11' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcConstraint.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcConstraint.WR11", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcConstraint
	{
		public static readonly IfcConstraint WR11 = new IfcConstraint();
		protected IfcConstraint() {}
	}
}
