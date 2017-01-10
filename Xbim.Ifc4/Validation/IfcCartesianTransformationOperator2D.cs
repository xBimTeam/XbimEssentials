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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcCartesianTransformationOperator2D : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCartesianTransformationOperator2D clause) {
			var retVal = false;
			if (clause == Where.IfcCartesianTransformationOperator2D.DimEqual2) {
				try {
					retVal = this/* as IfcCartesianTransformationOperator*/.Dim == 2;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator2D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator2D.DimEqual2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCartesianTransformationOperator2D.Axis1Is2D) {
				try {
					retVal = !(EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis1)) || (this/* as IfcCartesianTransformationOperator*/.Axis1.Dim == 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator2D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator2D.Axis1Is2D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCartesianTransformationOperator2D.Axis2Is2D) {
				try {
					retVal = !(EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis2)) || (this/* as IfcCartesianTransformationOperator*/.Axis2.Dim == 2);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator2D");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator2D.Axis2Is2D' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcCartesianTransformationOperator)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcCartesianTransformationOperator2D.DimEqual2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.DimEqual2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCartesianTransformationOperator2D.Axis1Is2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.Axis1Is2D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCartesianTransformationOperator2D.Axis2Is2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.Axis2Is2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCartesianTransformationOperator2D : IfcCartesianTransformationOperator
	{
		public static readonly IfcCartesianTransformationOperator2D DimEqual2 = new IfcCartesianTransformationOperator2D();
		public static readonly IfcCartesianTransformationOperator2D Axis1Is2D = new IfcCartesianTransformationOperator2D();
		public static readonly IfcCartesianTransformationOperator2D Axis2Is2D = new IfcCartesianTransformationOperator2D();
		protected IfcCartesianTransformationOperator2D() {}
	}
}
