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
namespace Xbim.Ifc2x3.ProfileResource
{
	public partial class IfcCompositeProfileDef : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCompositeProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcCompositeProfileDef.WR1) {
				try {
					retVal = SIZEOF(Profiles.Where(temp => temp.ProfileType != Profiles.ItemAt(0).ProfileType)) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcCompositeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCompositeProfileDef.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompositeProfileDef.WR2) {
				try {
					retVal = SIZEOF(Profiles.Where(temp => TYPEOF(temp).Contains("IFC2X3.IFCCOMPOSITEPROFILEDEF"))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcCompositeProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCompositeProfileDef.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCompositeProfileDef.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompositeProfileDef.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeProfileDef.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcCompositeProfileDef
	{
		public static readonly IfcCompositeProfileDef WR1 = new IfcCompositeProfileDef();
		public static readonly IfcCompositeProfileDef WR2 = new IfcCompositeProfileDef();
		protected IfcCompositeProfileDef() {}
	}
}
