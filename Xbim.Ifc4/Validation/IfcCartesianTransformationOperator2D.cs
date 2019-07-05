using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcCartesianTransformationOperator2D : IExpressValidatable
	{
		public enum IfcCartesianTransformationOperator2DClause
		{
			DimEqual2,
			Axis1Is2D,
			Axis2Is2D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCartesianTransformationOperator2DClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCartesianTransformationOperator2DClause.DimEqual2:
						retVal = this/* as IfcCartesianTransformationOperator*/.Dim == 2;
						break;
					case IfcCartesianTransformationOperator2DClause.Axis1Is2D:
						retVal = !(Functions.EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis1)) || (this/* as IfcCartesianTransformationOperator*/.Axis1.Dim == 2);
						break;
					case IfcCartesianTransformationOperator2DClause.Axis2Is2D:
						retVal = !(Functions.EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis2)) || (this/* as IfcCartesianTransformationOperator*/.Axis2.Dim == 2);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator2D>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator2D.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcCartesianTransformationOperator2DClause.DimEqual2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.DimEqual2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCartesianTransformationOperator2DClause.Axis1Is2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.Axis1Is2D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCartesianTransformationOperator2DClause.Axis2Is2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator2D.Axis2Is2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
