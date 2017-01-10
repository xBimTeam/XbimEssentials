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
namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
	public partial class IfcDimensionCurveDirectedCallout : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDimensionCurveDirectedCallout clause) {
			var retVal = false;
			if (clause == Where.IfcDimensionCurveDirectedCallout.WR41) {
				try {
					retVal = SIZEOF(this/* as IfcDraughtingCallout*/.Contents.Where(Dc => (TYPEOF(Dc).Contains("IFC2X3.IFCDIMENSIONCURVE")))) == 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCurveDirectedCallout");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCurveDirectedCallout.WR41' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcDimensionCurveDirectedCallout.WR42) {
				try {
					retVal = SIZEOF(this.Contents.Where(Dc => (TYPEOF(Dc).Contains("IFC2X3.IFCPROJECTIONCURVE")))) <= 2;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCurveDirectedCallout");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCurveDirectedCallout.WR42' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcDimensionCurveDirectedCallout.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurveDirectedCallout.WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcDimensionCurveDirectedCallout.WR42))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurveDirectedCallout.WR42", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcDimensionCurveDirectedCallout
	{
		public static readonly IfcDimensionCurveDirectedCallout WR41 = new IfcDimensionCurveDirectedCallout();
		public static readonly IfcDimensionCurveDirectedCallout WR42 = new IfcDimensionCurveDirectedCallout();
		protected IfcDimensionCurveDirectedCallout() {}
	}
}
