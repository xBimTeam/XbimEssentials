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
	public partial class IfcCartesianTransformationOperator2DnonUniform : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCartesianTransformationOperator2DnonUniform clause) {
			var retVal = false;
			if (clause == Where.IfcCartesianTransformationOperator2DnonUniform.Scale2GreaterZero) {
				try {
					retVal = Scl2 > 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator2DnonUniform");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator2DnonUniform.Scale2GreaterZero' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcCartesianTransformationOperator2D)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcCartesianTransformationOperator2DnonUniform.Scale2GreaterZero))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2DnonUniform.Scale2GreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCartesianTransformationOperator2DnonUniform : IfcCartesianTransformationOperator2D
	{
		public static readonly IfcCartesianTransformationOperator2DnonUniform Scale2GreaterZero = new IfcCartesianTransformationOperator2DnonUniform();
		protected IfcCartesianTransformationOperator2DnonUniform() {}
	}
}
