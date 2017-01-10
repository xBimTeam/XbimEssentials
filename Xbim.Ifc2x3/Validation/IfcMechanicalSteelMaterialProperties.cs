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
namespace Xbim.Ifc2x3.MaterialPropertyResource
{
	public partial class IfcMechanicalSteelMaterialProperties : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcMechanicalSteelMaterialProperties clause) {
			var retVal = false;
			if (clause == Where.IfcMechanicalSteelMaterialProperties.WR31) {
				try {
					retVal = !(EXISTS(YieldStress)) || (YieldStress >= 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MaterialPropertyResource.IfcMechanicalSteelMaterialProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMechanicalSteelMaterialProperties.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcMechanicalSteelMaterialProperties.WR32) {
				try {
					retVal = !(EXISTS(UltimateStress)) || (UltimateStress >= 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MaterialPropertyResource.IfcMechanicalSteelMaterialProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMechanicalSteelMaterialProperties.WR32' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcMechanicalSteelMaterialProperties.WR33) {
				try {
					retVal = !(EXISTS(HardeningModule)) || (HardeningModule >= 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MaterialPropertyResource.IfcMechanicalSteelMaterialProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMechanicalSteelMaterialProperties.WR33' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcMechanicalSteelMaterialProperties.WR34) {
				try {
					retVal = !(EXISTS(ProportionalStress)) || (ProportionalStress >= 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MaterialPropertyResource.IfcMechanicalSteelMaterialProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMechanicalSteelMaterialProperties.WR34' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcMechanicalMaterialProperties)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcMechanicalSteelMaterialProperties.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalSteelMaterialProperties.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcMechanicalSteelMaterialProperties.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalSteelMaterialProperties.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcMechanicalSteelMaterialProperties.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalSteelMaterialProperties.WR33", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcMechanicalSteelMaterialProperties.WR34))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalSteelMaterialProperties.WR34", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcMechanicalSteelMaterialProperties : IfcMechanicalMaterialProperties
	{
		public static readonly IfcMechanicalSteelMaterialProperties WR31 = new IfcMechanicalSteelMaterialProperties();
		public static readonly IfcMechanicalSteelMaterialProperties WR32 = new IfcMechanicalSteelMaterialProperties();
		public static readonly IfcMechanicalSteelMaterialProperties WR33 = new IfcMechanicalSteelMaterialProperties();
		public static readonly IfcMechanicalSteelMaterialProperties WR34 = new IfcMechanicalSteelMaterialProperties();
		protected IfcMechanicalSteelMaterialProperties() {}
	}
}
