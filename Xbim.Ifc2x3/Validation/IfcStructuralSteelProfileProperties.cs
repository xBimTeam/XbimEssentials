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
namespace Xbim.Ifc2x3.ProfilePropertyResource
{
	public partial class IfcStructuralSteelProfileProperties : IExpressValidatable
	{
		public enum IfcStructuralSteelProfilePropertiesClause
		{
			WR31,
			WR32,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralSteelProfilePropertiesClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralSteelProfilePropertiesClause.WR31:
						retVal = !(Functions.EXISTS(ShearAreaY)) || (ShearAreaY >= 0);
						break;
					case IfcStructuralSteelProfilePropertiesClause.WR32:
						retVal = !(Functions.EXISTS(ShearAreaZ)) || (ShearAreaZ >= 0);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProfilePropertyResource.IfcStructuralSteelProfileProperties>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSteelProfileProperties.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralSteelProfilePropertiesClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSteelProfileProperties.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcStructuralSteelProfilePropertiesClause.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSteelProfileProperties.WR32", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
