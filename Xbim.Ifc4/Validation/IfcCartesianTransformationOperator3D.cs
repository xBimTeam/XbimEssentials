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
	public partial class IfcCartesianTransformationOperator3D : IExpressValidatable
	{
		public enum IfcCartesianTransformationOperator3DClause
		{
			DimIs3D,
			Axis1Is3D,
			Axis2Is3D,
			Axis3Is3D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCartesianTransformationOperator3DClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCartesianTransformationOperator3DClause.DimIs3D:
						retVal = this/* as IfcCartesianTransformationOperator*/.Dim == 3;
						break;
					case IfcCartesianTransformationOperator3DClause.Axis1Is3D:
						retVal = !(Functions.EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis1)) || (this/* as IfcCartesianTransformationOperator*/.Axis1.Dim == 3);
						break;
					case IfcCartesianTransformationOperator3DClause.Axis2Is3D:
						retVal = !(Functions.EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis2)) || (this/* as IfcCartesianTransformationOperator*/.Axis2.Dim == 3);
						break;
					case IfcCartesianTransformationOperator3DClause.Axis3Is3D:
						retVal = !(Functions.EXISTS(Axis3)) || (Axis3.Dim == 3);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcCartesianTransformationOperator3D>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator3D.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcCartesianTransformationOperator3DClause.DimIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.DimIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCartesianTransformationOperator3DClause.Axis1Is3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.Axis1Is3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCartesianTransformationOperator3DClause.Axis2Is3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.Axis2Is3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCartesianTransformationOperator3DClause.Axis3Is3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.Axis3Is3D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
