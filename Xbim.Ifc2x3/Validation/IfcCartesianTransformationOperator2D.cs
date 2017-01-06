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
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class IfcCartesianTransformationOperator2D : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcCartesianTransformationOperator2D");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCartesianTransformationOperator2D clause) {
			var retVal = false;
			if (clause == Where.IfcCartesianTransformationOperator2D.WR1) {
				try {
					retVal = this/* as IfcCartesianTransformationOperator*/.Dim == 2;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator2D.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCartesianTransformationOperator2D.WR2) {
				try {
					retVal = !(EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis1)) || (this/* as IfcCartesianTransformationOperator*/.Axis1.Dim == 2);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator2D.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCartesianTransformationOperator2D.WR3) {
				try {
					retVal = !(EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis2)) || (this/* as IfcCartesianTransformationOperator*/.Axis2.Dim == 2);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator2D.WR3' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcCartesianTransformationOperator)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcCartesianTransformationOperator2D.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCartesianTransformationOperator2D.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCartesianTransformationOperator2D.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcCartesianTransformationOperator2D : IfcCartesianTransformationOperator
	{
		public new static readonly IfcCartesianTransformationOperator2D WR1 = new IfcCartesianTransformationOperator2D();
		public static readonly IfcCartesianTransformationOperator2D WR2 = new IfcCartesianTransformationOperator2D();
		public static readonly IfcCartesianTransformationOperator2D WR3 = new IfcCartesianTransformationOperator2D();
		protected IfcCartesianTransformationOperator2D() {}
	}
}
