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
namespace Xbim.Ifc4.ConstraintResource
{
	public partial class IfcObjective : IExpressValidatable
	{
		public enum IfcObjectiveClause
		{
			WR21,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcObjectiveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcObjectiveClause.WR21:
						retVal = (ObjectiveQualifier != IfcObjectiveEnum.USERDEFINED) || ((ObjectiveQualifier == IfcObjectiveEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObjective*/.UserDefinedQualifier));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ConstraintResource.IfcObjective>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcObjective.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcObjectiveClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcObjective.WR21", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
