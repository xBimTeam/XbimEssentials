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
	public partial class IfcCartesianTransformationOperator3DnonUniform : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcCartesianTransformationOperator3DnonUniform");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCartesianTransformationOperator3DnonUniform clause) {
			var retVal = false;
			if (clause == Where.IfcCartesianTransformationOperator3DnonUniform.WR1) {
				try {
					retVal = Scl2 > 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator3DnonUniform.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCartesianTransformationOperator3DnonUniform.WR2) {
				try {
					retVal = Scl3 > 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator3DnonUniform.WR2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcCartesianTransformationOperator3D)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcCartesianTransformationOperator3DnonUniform.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3DnonUniform.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCartesianTransformationOperator3DnonUniform.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3DnonUniform.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcCartesianTransformationOperator3DnonUniform : IfcCartesianTransformationOperator3D
	{
		public new static readonly IfcCartesianTransformationOperator3DnonUniform WR1 = new IfcCartesianTransformationOperator3DnonUniform();
		public new static readonly IfcCartesianTransformationOperator3DnonUniform WR2 = new IfcCartesianTransformationOperator3DnonUniform();
		protected IfcCartesianTransformationOperator3DnonUniform() {}
	}
}
