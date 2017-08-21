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
namespace Xbim.Ifc2x3.GeometricConstraintResource
{
	public partial class IfcLocalPlacement : IExpressValidatable
	{
		public enum IfcLocalPlacementClause
		{
			WR21,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcLocalPlacementClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcLocalPlacementClause.WR21:
						retVal = Functions.IfcCorrectLocalPlacement(RelativePlacement, PlacementRelTo);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometricConstraintResource.IfcLocalPlacement>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcLocalPlacement.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcLocalPlacementClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcLocalPlacement.WR21", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
