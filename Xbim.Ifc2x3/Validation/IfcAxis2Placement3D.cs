using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class IfcAxis2Placement3D : IExpressValidatable
	{
		public enum IfcAxis2Placement3DClause
		{
			WR1,
			WR2,
			WR3,
			WR4,
			WR5,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAxis2Placement3DClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAxis2Placement3DClause.WR1:
						retVal = this/* as IfcPlacement*/.Location.Dim == 3;
						break;
					case IfcAxis2Placement3DClause.WR2:
						retVal = (!(Functions.EXISTS(Axis))) || (Axis.Dim == 3);
						break;
					case IfcAxis2Placement3DClause.WR3:
						retVal = (!(Functions.EXISTS(RefDirection))) || (RefDirection.Dim == 3);
						break;
					case IfcAxis2Placement3DClause.WR4:
						retVal = (!(Functions.EXISTS(Axis))) || (!(Functions.EXISTS(RefDirection))) || (Functions.IfcCrossProduct(Axis, RefDirection).Magnitude > 0);
						break;
					case IfcAxis2Placement3DClause.WR5:
						retVal = !((Functions.EXISTS(Axis)) ^ (Functions.EXISTS(RefDirection)));
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometryResource.IfcAxis2Placement3D>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement3D.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcAxis2Placement3DClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement3DClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement3DClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR3", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement3DClause.WR4))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR4", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement3DClause.WR5))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.WR5", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
