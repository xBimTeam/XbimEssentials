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
	public partial class IfcCompositeCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.GeometryResource.IfcCompositeCurve");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCompositeCurve clause) {
			var retVal = false;
			if (clause == Where.IfcCompositeCurve.WR41) {
				try {
					retVal = ((!ClosedCurve.Value) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 1)) || ((ClosedCurve.Value) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 0));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCompositeCurve.WR41' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompositeCurve.WR42) {
				try {
					retVal = SIZEOF(Segments.Where(Temp => Temp.Dim != Segments.ToArray()[0].Dim)) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCompositeCurve.WR42' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCompositeCurve.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeCurve.WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompositeCurve.WR42))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeCurve.WR42", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcCompositeCurve
	{
		public static readonly IfcCompositeCurve WR41 = new IfcCompositeCurve();
		public static readonly IfcCompositeCurve WR42 = new IfcCompositeCurve();
		protected IfcCompositeCurve() {}
	}
}
