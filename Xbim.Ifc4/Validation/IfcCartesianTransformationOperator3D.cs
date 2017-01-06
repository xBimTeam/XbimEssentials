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
	public partial class IfcCartesianTransformationOperator3D : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator3D");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCartesianTransformationOperator3D clause) {
			var retVal = false;
			if (clause == Where.IfcCartesianTransformationOperator3D.DimIs3D) {
				try {
					retVal = this/* as IfcCartesianTransformationOperator*/.Dim == 3;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator3D.DimIs3D' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCartesianTransformationOperator3D.Axis1Is3D) {
				try {
					retVal = !(EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis1)) || (this/* as IfcCartesianTransformationOperator*/.Axis1.Dim == 3);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator3D.Axis1Is3D' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCartesianTransformationOperator3D.Axis2Is3D) {
				try {
					retVal = !(EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis2)) || (this/* as IfcCartesianTransformationOperator*/.Axis2.Dim == 3);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator3D.Axis2Is3D' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCartesianTransformationOperator3D.Axis3Is3D) {
				try {
					retVal = !(EXISTS(Axis3)) || (Axis3.Dim == 3);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator3D.Axis3Is3D' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcCartesianTransformationOperator3D.DimIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.DimIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCartesianTransformationOperator3D.Axis1Is3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.Axis1Is3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCartesianTransformationOperator3D.Axis2Is3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.Axis2Is3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCartesianTransformationOperator3D.Axis3Is3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.Axis3Is3D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCartesianTransformationOperator3D : IfcCartesianTransformationOperator
	{
		public static readonly IfcCartesianTransformationOperator3D DimIs3D = new IfcCartesianTransformationOperator3D();
		public static readonly IfcCartesianTransformationOperator3D Axis1Is3D = new IfcCartesianTransformationOperator3D();
		public static readonly IfcCartesianTransformationOperator3D Axis2Is3D = new IfcCartesianTransformationOperator3D();
		public static readonly IfcCartesianTransformationOperator3D Axis3Is3D = new IfcCartesianTransformationOperator3D();
		protected IfcCartesianTransformationOperator3D() {}
	}
}
