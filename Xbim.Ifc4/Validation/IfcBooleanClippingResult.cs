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
		/// Tests the express where clause FirstOperandType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool FirstOperandType() {
			var retVal = false;
			try {
				retVal = (TYPEOF(FirstOperand).Contains("IFC4.IFCSWEPTAREASOLID")) || (TYPEOF(FirstOperand).Contains("IFC4.IFCSWEPTDISCSOLID")) || (TYPEOF(FirstOperand).Contains("IFC4.IFCBOOLEANCLIPPINGRESULT"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'FirstOperandType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause SecondOperandType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SecondOperandType() {
			var retVal = false;
			try {
				retVal = (TYPEOF(SecondOperand).Contains("IFC4.IFCHALFSPACESOLID"));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SecondOperandType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause OperatorType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool OperatorType() {
			var retVal = false;
			try {
				retVal = Operator == IfcBooleanOperator.DIFFERENCE;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'OperatorType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!FirstOperandType())
				yield return new ValidationResult() { Item = this, IssueSource = "FirstOperandType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!SecondOperandType())
				yield return new ValidationResult() { Item = this, IssueSource = "SecondOperandType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!OperatorType())
				yield return new ValidationResult() { Item = this, IssueSource = "OperatorType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
