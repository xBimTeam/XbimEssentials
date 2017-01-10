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
namespace Xbim.Ifc2x3.ProfilePropertyResource
{
	public partial class IfcStructuralSteelProfileProperties : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralSteelProfileProperties clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralSteelProfileProperties.WR31) {
				try {
					retVal = !(EXISTS(ShearAreaY)) || (ShearAreaY >= 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfilePropertyResource.IfcStructuralSteelProfileProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSteelProfileProperties.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralSteelProfileProperties.WR32) {
				try {
					retVal = !(EXISTS(ShearAreaZ)) || (ShearAreaZ >= 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfilePropertyResource.IfcStructuralSteelProfileProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralSteelProfileProperties.WR32' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcStructuralProfileProperties)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralSteelProfileProperties.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSteelProfileProperties.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralSteelProfileProperties.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralSteelProfileProperties.WR32", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcStructuralSteelProfileProperties : IfcStructuralProfileProperties
	{
		public static readonly IfcStructuralSteelProfileProperties WR31 = new IfcStructuralSteelProfileProperties();
		public static readonly IfcStructuralSteelProfileProperties WR32 = new IfcStructuralSteelProfileProperties();
		protected IfcStructuralSteelProfileProperties() {}
	}
}
