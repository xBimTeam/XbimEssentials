using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.GeometricModelResource
{
	public partial class IfcBooleanClippingResult : IExpressValidatable
	{
		public enum IfcBooleanClippingResultClause
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
		public bool ValidateClause(IfcBooleanClippingResultClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBooleanClippingResultClause.WR1:
						retVal = (Functions.TYPEOF(FirstOperand).Contains("IFC2X3.IFCSWEPTAREASOLID")) || (Functions.TYPEOF(FirstOperand).Contains("IFC2X3.IFCBOOLEANCLIPPINGRESULT"));
						break;
					case IfcBooleanClippingResultClause.WR2:
						retVal = (Functions.TYPEOF(SecondOperand).Contains("IFC2X3.IFCHALFSPACESOLID"));
						break;
					case IfcBooleanClippingResultClause.WR3:
						retVal = Operator == IfcBooleanOperator.DIFFERENCE;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometricModelResource.IfcBooleanClippingResult>();
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
			if (!ValidateClause(IfcBooleanClippingResultClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBooleanClippingResultClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcBooleanClippingResultClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
