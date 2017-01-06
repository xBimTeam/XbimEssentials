using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcCompositeCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCompositeCurve");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcCompositeCurve clause) {
			var retVal = false;
			if (clause == Where.IfcCompositeCurve.CurveContinuous) {
				try {
					retVal = ((!ClosedCurve.AsBool()) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 1)) || ((ClosedCurve.AsBool()) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 0));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCompositeCurve.CurveContinuous' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcCompositeCurve.SameDim) {
				try {
					retVal = SIZEOF(Segments.Where(Temp => Temp.Dim != Segments.ToArray()[0].Dim)) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcCompositeCurve.SameDim' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcCompositeCurve.CurveContinuous))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeCurve.CurveContinuous", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcCompositeCurve.SameDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeCurve.SameDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcCompositeCurve
	{
		public static readonly IfcCompositeCurve CurveContinuous = new IfcCompositeCurve();
		public static readonly IfcCompositeCurve SameDim = new IfcCompositeCurve();
		protected IfcCompositeCurve() {}
	}
}
