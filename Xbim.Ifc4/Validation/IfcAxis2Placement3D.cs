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
	public partial class IfcAxis2Placement3D : IExpressValidatable
	{
		public enum IfcAxis2Placement3DClause
		{
			LocationIs3D,
			AxisIs3D,
			RefDirIs3D,
			AxisToRefDirPosition,
			AxisAndRefDirProvision,
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
					case IfcAxis2Placement3DClause.LocationIs3D:
						retVal = this/* as IfcPlacement*/.Location.Dim == 3;
						break;
					case IfcAxis2Placement3DClause.AxisIs3D:
						retVal = (!(Functions.EXISTS(Axis))) || (Axis.Dim == 3);
						break;
					case IfcAxis2Placement3DClause.RefDirIs3D:
						retVal = (!(Functions.EXISTS(RefDirection))) || (RefDirection.Dim == 3);
						break;
					case IfcAxis2Placement3DClause.AxisToRefDirPosition:
						retVal = (!(Functions.EXISTS(Axis))) || (!(Functions.EXISTS(RefDirection))) || (Functions.IfcCrossProduct(Axis, RefDirection).Magnitude > 0);
						break;
					case IfcAxis2Placement3DClause.AxisAndRefDirProvision:
						retVal = !((Functions.EXISTS(Axis)) ^ (Functions.EXISTS(RefDirection)));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcAxis2Placement3D>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAxis2Placement3D.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcAxis2Placement3DClause.LocationIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.LocationIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement3DClause.AxisIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.AxisIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement3DClause.RefDirIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.RefDirIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement3DClause.AxisToRefDirPosition))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.AxisToRefDirPosition", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis2Placement3DClause.AxisAndRefDirProvision))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis2Placement3D.AxisAndRefDirProvision", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
