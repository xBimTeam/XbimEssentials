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
		public enum IfcCompositeCurveClause
		{
			CurveContinuous,
			SameDim,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCompositeCurveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCompositeCurveClause.CurveContinuous:
						retVal = ((!ClosedCurve.AsBool()) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 1)) || ((ClosedCurve.AsBool()) && (SIZEOF(Segments.Where(Temp => Temp.Transition == IfcTransitionCode.DISCONTINUOUS)) == 0));
						break;
					case IfcCompositeCurveClause.SameDim:
						retVal = SIZEOF(Segments.Where(Temp => Temp.Dim != Segments.ItemAt(0).Dim)) == 0;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcCompositeCurve");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCompositeCurve.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCompositeCurveClause.CurveContinuous))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeCurve.CurveContinuous", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCompositeCurveClause.SameDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeCurve.SameDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
