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
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class IfcSweptSurface : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSweptSurface clause) {
			var retVal = false;
			if (clause == Where.IfcSweptSurface.WR1) {
				try {
					retVal = !(TYPEOF(SweptCurve).Contains("IFC2X3.IFCDERIVEDPROFILEDEF"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcSweptSurface");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSweptSurface.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSweptSurface.WR2) {
				try {
					retVal = SweptCurve.ProfileType == IfcProfileTypeEnum.CURVE;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcSweptSurface");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSweptSurface.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSweptSurface.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptSurface.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSweptSurface.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptSurface.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcSweptSurface
	{
		public static readonly IfcSweptSurface WR1 = new IfcSweptSurface();
		public static readonly IfcSweptSurface WR2 = new IfcSweptSurface();
		protected IfcSweptSurface() {}
	}
}
