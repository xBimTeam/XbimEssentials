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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcBooleanClippingResult : IExpressValidatable
	{
		public enum IfcBooleanClippingResultClause
		{
			FirstOperandType,
			SecondOperandType,
			OperatorType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcBooleanClippingResultClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBooleanClippingResultClause.FirstOperandType:
						retVal = (Functions.TYPEOF(FirstOperand).Contains("IFC4.IFCSWEPTAREASOLID")) || (Functions.TYPEOF(FirstOperand).Contains("IFC4.IFCSWEPTDISCSOLID")) || (Functions.TYPEOF(FirstOperand).Contains("IFC4.IFCBOOLEANCLIPPINGRESULT"));
						break;
					case IfcBooleanClippingResultClause.SecondOperandType:
						retVal = (Functions.TYPEOF(SecondOperand).Contains("IFC4.IFCHALFSPACESOLID"));
						break;
					case IfcBooleanClippingResultClause.OperatorType:
						retVal = Operator == IfcBooleanOperator.DIFFERENCE;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcBooleanClippingResult>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcBooleanClippingResult.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcBooleanClippingResultClause.FirstOperandType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.FirstOperandType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBooleanClippingResultClause.SecondOperandType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.SecondOperandType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBooleanClippingResultClause.OperatorType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.OperatorType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
