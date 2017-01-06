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
	public partial class IfcCartesianTransformationOperator : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcCartesianTransformationOperator");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCartesianTransformationOperator clause) {
			var retVal = false;
			if (clause == Where.IfcCartesianTransformationOperator.WR1) {
				try {
					retVal = Scl > 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCartesianTransformationOperator.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcCartesianTransformationOperator
	{
		public static readonly IfcCartesianTransformationOperator WR1 = new IfcCartesianTransformationOperator();
		protected IfcCartesianTransformationOperator() {}
	}
}
