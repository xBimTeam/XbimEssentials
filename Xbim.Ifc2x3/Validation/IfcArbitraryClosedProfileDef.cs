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
	public partial class IfcArbitraryClosedProfileDef : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcArbitraryClosedProfileDef clause) {
			var retVal = false;
			if (clause == Where.IfcArbitraryClosedProfileDef.WR1) {
				try {
					retVal = OuterCurve.Dim == 2;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcArbitraryClosedProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcArbitraryClosedProfileDef.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcArbitraryClosedProfileDef.WR2) {
				try {
					retVal = !(TYPEOF(OuterCurve).Contains("IFC2X3.IFCLINE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcArbitraryClosedProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcArbitraryClosedProfileDef.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcArbitraryClosedProfileDef.WR3) {
				try {
					retVal = !(TYPEOF(OuterCurve).Contains("IFC2X3.IFCOFFSETCURVE2D"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcArbitraryClosedProfileDef");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcArbitraryClosedProfileDef.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcArbitraryClosedProfileDef.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryClosedProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcArbitraryClosedProfileDef.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryClosedProfileDef.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcArbitraryClosedProfileDef.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcArbitraryClosedProfileDef.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcArbitraryClosedProfileDef
	{
		public static readonly IfcArbitraryClosedProfileDef WR1 = new IfcArbitraryClosedProfileDef();
		public static readonly IfcArbitraryClosedProfileDef WR2 = new IfcArbitraryClosedProfileDef();
		public static readonly IfcArbitraryClosedProfileDef WR3 = new IfcArbitraryClosedProfileDef();
		protected IfcArbitraryClosedProfileDef() {}
	}
}
