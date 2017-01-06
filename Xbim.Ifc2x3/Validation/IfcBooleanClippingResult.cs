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
namespace Xbim.Ifc2x3.GeometricModelResource
{
	public partial class IfcBooleanClippingResult : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometricModelResource.IfcBooleanClippingResult");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBooleanClippingResult clause) {
			var retVal = false;
			if (clause == Where.IfcBooleanClippingResult.WR1) {
				try {
					retVal = (TYPEOF(FirstOperand).Contains("IFC2X3.IFCSWEPTAREASOLID")) || (TYPEOF(FirstOperand).Contains("IFC2X3.IFCBOOLEANCLIPPINGRESULT"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBooleanClippingResult.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBooleanClippingResult.WR2) {
				try {
					retVal = (TYPEOF(SecondOperand).Contains("IFC2X3.IFCHALFSPACESOLID"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBooleanClippingResult.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBooleanClippingResult.WR3) {
				try {
					retVal = Operator == IfcBooleanOperator.DIFFERENCE;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBooleanClippingResult.WR3' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcBooleanClippingResult.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBooleanClippingResult.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBooleanClippingResult.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBooleanClippingResult.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcBooleanClippingResult : IfcBooleanResult
	{
		public new static readonly IfcBooleanClippingResult WR1 = new IfcBooleanClippingResult();
		public static readonly IfcBooleanClippingResult WR2 = new IfcBooleanClippingResult();
		public static readonly IfcBooleanClippingResult WR3 = new IfcBooleanClippingResult();
		protected IfcBooleanClippingResult() {}
	}
}
