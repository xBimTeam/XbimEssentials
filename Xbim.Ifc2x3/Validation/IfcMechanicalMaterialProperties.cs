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
	public partial class IfcMechanicalMaterialProperties : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MaterialPropertyResource.IfcMechanicalMaterialProperties");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcMechanicalMaterialProperties clause) {
			var retVal = false;
			if (clause == Where.IfcMechanicalMaterialProperties.WR21) {
				try {
					retVal = !(EXISTS(YoungModulus)) || (YoungModulus >= 0);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcMechanicalMaterialProperties.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcMechanicalMaterialProperties.WR22) {
				try {
					retVal = !(EXISTS(ShearModulus)) || (ShearModulus >= 0);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcMechanicalMaterialProperties.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcMechanicalMaterialProperties.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalMaterialProperties.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcMechanicalMaterialProperties.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMechanicalMaterialProperties.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcMechanicalMaterialProperties
	{
		public static readonly IfcMechanicalMaterialProperties WR21 = new IfcMechanicalMaterialProperties();
		public static readonly IfcMechanicalMaterialProperties WR22 = new IfcMechanicalMaterialProperties();
		protected IfcMechanicalMaterialProperties() {}
	}
}
