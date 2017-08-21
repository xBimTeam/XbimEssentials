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
	public partial class IfcAxis1Placement : IExpressValidatable
	{
		public enum IfcAxis1PlacementClause
		{
			WR1,
			WR2,
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
					case IfcAxis1PlacementClause.WR1:
						retVal = (!(Functions.EXISTS(Axis))) || (Axis.Dim == 3);
						break;
					case IfcAxis1PlacementClause.WR2:
						retVal = this/* as IfcPlacement*/.Location.Dim == 3;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometryResource.IfcAxis1Placement>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAxis1Placement.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcAxis1PlacementClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis1Placement.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcAxis1PlacementClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAxis1Placement.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
