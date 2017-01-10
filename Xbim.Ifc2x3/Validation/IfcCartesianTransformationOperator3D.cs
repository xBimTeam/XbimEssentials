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
	public partial class IfcCartesianTransformationOperator3D : IExpressValidatable
	{
		public enum IfcCartesianTransformationOperator3DClause
		{
			WR1,
			WR2,
			WR3,
			WR4,
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
					case IfcCartesianTransformationOperator3DClause.WR1:
						retVal = this/* as IfcCartesianTransformationOperator*/.Dim == 3;
						break;
					case IfcCartesianTransformationOperator3DClause.WR2:
						retVal = !(EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis1)) || (this/* as IfcCartesianTransformationOperator*/.Axis1.Dim == 3);
						break;
					case IfcCartesianTransformationOperator3DClause.WR3:
						retVal = !(EXISTS(this/* as IfcCartesianTransformationOperator*/.Axis2)) || (this/* as IfcCartesianTransformationOperator*/.Axis2.Dim == 3);
						break;
					case IfcCartesianTransformationOperator3DClause.WR4:
						retVal = !(EXISTS(Axis3)) || (Axis3.Dim == 3);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcCartesianTransformationOperator3D");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCartesianTransformationOperator3D.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcCartesianTransformationOperator3DClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCartesianTransformationOperator3DClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCartesianTransformationOperator3DClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCartesianTransformationOperator3DClause.WR4))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianTransformationOperator3D.WR4", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
