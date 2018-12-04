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
namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	public partial class IfcStructuralLinearAction : IExpressValidatable
	{
		public enum IfcStructuralLinearActionClause
		{
			SuitableLoadType,
			ConstPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralLinearActionClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralLinearActionClause.SuitableLoadType:
						retVal = Functions.SIZEOF(Functions.NewTypesArray("IFC4.IFCSTRUCTURALLOADLINEARFORCE", "IFC4.IFCSTRUCTURALLOADTEMPERATURE") * Functions.TYPEOF(this/* as IfcStructuralActivity*/.AppliedLoad)) == 1;
						break;
					case IfcStructuralLinearActionClause.ConstPredefinedType:
						retVal = this/* as IfcStructuralCurveAction*/.PredefinedType == IfcStructuralCurveActivityTypeEnum.CONST;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralLinearAction>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStructuralLinearAction.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralLinearActionClause.SuitableLoadType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralLinearAction.SuitableLoadType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcStructuralLinearActionClause.ConstPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralLinearAction.ConstPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
