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
namespace Xbim.Ifc2x3.MaterialPropertyResource
{
	public partial class IfcMechanicalSteelMaterialProperties : IExpressValidatable
	{
		public enum IfcMechanicalSteelMaterialPropertiesClause
		{
			WR31,
			WR32,
			WR33,
			WR34,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcMechanicalSteelMaterialPropertiesClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcMechanicalSteelMaterialPropertiesClause.WR31:
						retVal = !(Functions.EXISTS(YieldStress)) || (YieldStress >= 0);
						break;
					case IfcMechanicalSteelMaterialPropertiesClause.WR32:
						retVal = !(Functions.EXISTS(UltimateStress)) || (UltimateStress >= 0);
						break;
					case IfcMechanicalSteelMaterialPropertiesClause.WR33:
						retVal = !(Functions.EXISTS(HardeningModule)) || (HardeningModule >= 0);
						break;
					case IfcMechanicalSteelMaterialPropertiesClause.WR34:
						retVal = !(Functions.EXISTS(ProportionalStress)) || (ProportionalStress >= 0);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.MaterialPropertyResource.IfcMechanicalSteelMaterialProperties>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcMechanicalSteelMaterialProperties.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcMechanicalSteelMaterialPropertiesClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalSteelMaterialProperties.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcMechanicalSteelMaterialPropertiesClause.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalSteelMaterialProperties.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcMechanicalSteelMaterialPropertiesClause.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalSteelMaterialProperties.WR33", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcMechanicalSteelMaterialPropertiesClause.WR34))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalSteelMaterialProperties.WR34", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
