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
		public enum IfcDimensionCurveClause
		{
			WR51,
			WR52,
			WR53,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDimensionCurveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDimensionCurveClause.WR51:
						retVal = SIZEOF(USEDIN(this, "IFC2X3.IFCDRAUGHTINGCALLOUT.CONTENTS")) >= 1;
						break;
					case IfcDimensionCurveClause.WR52:
						retVal = (SIZEOF(USEDIN(this, "IFC2X3." + "IFCTERMINATORSYMBOL.ANNOTATEDCURVE").Where(Dct1 => (Dct1.AsIfcDimensionCurveTerminator().Role == IfcDimensionExtentUsage.ORIGIN))) <= 1) && (SIZEOF(USEDIN(this, "IFC2X3." + "IFCTERMINATORSYMBOL.ANNOTATEDCURVE").Where(Dct2 => (Dct2.AsIfcDimensionCurveTerminator().Role == IfcDimensionExtentUsage.TARGET))) <= 1);
						break;
					case IfcDimensionCurveClause.WR53:
						retVal = SIZEOF(AnnotatedBySymbols.Where(Dct => !(TYPEOF(Dct).Contains("IFC2X3.IFCDIMENSIONCURVETERMINATOR")))) == 0;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCurve");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCurve.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcDimensionCurveClause.WR51))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurve.WR51", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDimensionCurveClause.WR52))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurve.WR52", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDimensionCurveClause.WR53))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurve.WR53", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
