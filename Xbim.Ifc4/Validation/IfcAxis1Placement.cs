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
	public partial class IfcAxis1Placement : IExpressValidatable
	{
		public enum IfcAxis1PlacementClause
		{
			AxisIs3D,
			LocationIs3D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAxis1PlacementClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAxis1PlacementClause.AxisIs3D:
						retVal = (!(Functions.EXISTS(Axis))) || (Axis.Dim == 3);
						break;
					case IfcAxis1PlacementClause.LocationIs3D:
						retVal = this/* as IfcPlacement*/.Location.Dim == 3;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcAxis1Placement>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAxis1Placement.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcAxis1PlacementClause.AxisIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis1Placement.AxisIs3D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis1PlacementClause.LocationIs3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis1Placement.LocationIs3D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
