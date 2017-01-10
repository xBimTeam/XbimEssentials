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
	public partial class IfcStructuralProfileProperties : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStructuralProfileProperties clause) {
			var retVal = false;
			if (clause == Where.IfcStructuralProfileProperties.WR21) {
				try {
					retVal = !(EXISTS(ShearDeformationAreaY)) || (ShearDeformationAreaY >= 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfilePropertyResource.IfcStructuralProfileProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralProfileProperties.WR21' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcStructuralProfileProperties.WR22) {
				try {
					retVal = !(EXISTS(ShearDeformationAreaZ)) || (ShearDeformationAreaZ >= 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfilePropertyResource.IfcStructuralProfileProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralProfileProperties.WR22' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcGeneralProfileProperties)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcStructuralProfileProperties.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralProfileProperties.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcStructuralProfileProperties.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralProfileProperties.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcStructuralProfileProperties : IfcGeneralProfileProperties
	{
		public static readonly IfcStructuralProfileProperties WR21 = new IfcStructuralProfileProperties();
		public static readonly IfcStructuralProfileProperties WR22 = new IfcStructuralProfileProperties();
		protected IfcStructuralProfileProperties() {}
	}
}
