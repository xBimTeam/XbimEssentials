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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcBooleanClippingResult : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcBooleanClippingResult");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBooleanClippingResult clause) {
			var retVal = false;
			if (clause == Where.IfcBooleanClippingResult.FirstOperandType) {
				try {
					retVal = (TYPEOF(FirstOperand).Contains("IFC4.IFCSWEPTAREASOLID")) || (TYPEOF(FirstOperand).Contains("IFC4.IFCSWEPTDISCSOLID")) || (TYPEOF(FirstOperand).Contains("IFC4.IFCBOOLEANCLIPPINGRESULT"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBooleanClippingResult.FirstOperandType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBooleanClippingResult.SecondOperandType) {
				try {
					retVal = (TYPEOF(SecondOperand).Contains("IFC4.IFCHALFSPACESOLID"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBooleanClippingResult.SecondOperandType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBooleanClippingResult.OperatorType) {
				try {
					retVal = Operator == IfcBooleanOperator.DIFFERENCE;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBooleanClippingResult.OperatorType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcBooleanResult)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcBooleanClippingResult.FirstOperandType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.FirstOperandType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBooleanClippingResult.SecondOperandType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.SecondOperandType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBooleanClippingResult.OperatorType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.OperatorType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcBooleanClippingResult : IfcBooleanResult
	{
		public static readonly IfcBooleanClippingResult FirstOperandType = new IfcBooleanClippingResult();
		public static readonly IfcBooleanClippingResult SecondOperandType = new IfcBooleanClippingResult();
		public static readonly IfcBooleanClippingResult OperatorType = new IfcBooleanClippingResult();
		protected IfcBooleanClippingResult() {}
	}
}
