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
	public partial class IfcDimensionCurve : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCurve");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDimensionCurve clause) {
			var retVal = false;
			if (clause == Where.IfcDimensionCurve.WR51) {
				try {
					retVal = SIZEOF(USEDIN(this, "IFC2X3.IFCDRAUGHTINGCALLOUT.CONTENTS")) >= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDimensionCurve.WR51' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcDimensionCurve.WR52) {
				try {
					retVal = (SIZEOF(USEDIN(this, "IFC2X3." + "IFCTERMINATORSYMBOL.ANNOTATEDCURVE").Where(Dct1 => (Dct1.AsIfcDimensionCurveTerminator().Role == IfcDimensionExtentUsage.ORIGIN))) <= 1) && (SIZEOF(USEDIN(this, "IFC2X3." + "IFCTERMINATORSYMBOL.ANNOTATEDCURVE").Where(Dct2 => (Dct2.AsIfcDimensionCurveTerminator().Role == IfcDimensionExtentUsage.TARGET))) <= 1);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDimensionCurve.WR52' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcDimensionCurve.WR53) {
				try {
					retVal = SIZEOF(AnnotatedBySymbols.Where(Dct => !(TYPEOF(Dct).Contains("IFC2X3.IFCDIMENSIONCURVETERMINATOR")))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcDimensionCurve.WR53' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcAnnotationCurveOccurrence)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcDimensionCurve.WR51))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurve.WR51", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcDimensionCurve.WR52))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurve.WR52", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcDimensionCurve.WR53))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurve.WR53", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcDimensionCurve : IfcAnnotationCurveOccurrence
	{
		public static readonly IfcDimensionCurve WR51 = new IfcDimensionCurve();
		public static readonly IfcDimensionCurve WR52 = new IfcDimensionCurve();
		public static readonly IfcDimensionCurve WR53 = new IfcDimensionCurve();
		protected IfcDimensionCurve() {}
	}
}
