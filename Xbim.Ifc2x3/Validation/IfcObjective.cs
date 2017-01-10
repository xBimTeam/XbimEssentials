using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.ConstraintResource
{
	public partial class IfcObjective : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcObjective clause) {
			var retVal = false;
			if (clause == Where.IfcObjective.WR21) {
				try {
					retVal = (ObjectiveQualifier != IfcObjectiveEnum.USERDEFINED) || ((ObjectiveQualifier == IfcObjectiveEnum.USERDEFINED) && EXISTS(this/* as IfcObjective*/.UserDefinedQualifier));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ConstraintResource.IfcObjective");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcObjective.WR21' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcConstraint)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcObjective.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcObjective.WR21", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcObjective : IfcConstraint
	{
		public static readonly IfcObjective WR21 = new IfcObjective();
		protected IfcObjective() {}
	}
}
