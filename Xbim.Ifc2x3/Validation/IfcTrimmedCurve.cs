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
	public partial class IfcTrimmedCurve : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTrimmedCurve clause) {
			var retVal = false;
			if (clause == Where.IfcTrimmedCurve.WR41) {
				try {
					retVal = (HIINDEX(Trim1) == 1) || (TYPEOF(Trim1.ItemAt(0)) != TYPEOF(Trim1.ItemAt(1)));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcTrimmedCurve");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTrimmedCurve.WR41' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTrimmedCurve.WR42) {
				try {
					retVal = (HIINDEX(Trim2) == 1) || (TYPEOF(Trim2.ItemAt(0)) != TYPEOF(Trim2.ItemAt(1)));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcTrimmedCurve");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTrimmedCurve.WR42' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTrimmedCurve.WR43) {
				try {
					retVal = !(TYPEOF(BasisCurve).Contains("IFC2X3.IFCBOUNDEDCURVE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcTrimmedCurve");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTrimmedCurve.WR43' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTrimmedCurve.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTrimmedCurve.WR42))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.WR42", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTrimmedCurve.WR43))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.WR43", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcTrimmedCurve
	{
		public static readonly IfcTrimmedCurve WR41 = new IfcTrimmedCurve();
		public static readonly IfcTrimmedCurve WR42 = new IfcTrimmedCurve();
		public static readonly IfcTrimmedCurve WR43 = new IfcTrimmedCurve();
		protected IfcTrimmedCurve() {}
	}
}
