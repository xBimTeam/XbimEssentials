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
	public partial class IfcDimensionCurveTerminator : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDimensionCurveTerminator clause) {
			var retVal = false;
			if (clause == Where.IfcDimensionCurveTerminator.WR61) {
				try {
					retVal = TYPEOF(this/* as IfcTerminatorSymbol*/.AnnotatedCurve).Contains("IFC2X3.IFCDIMENSIONCURVE");
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCurveTerminator");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCurveTerminator.WR61' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcAnnotationSymbolOccurrence)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcDimensionCurveTerminator.WR61))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurveTerminator.WR61", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcDimensionCurveTerminator : IfcAnnotationSymbolOccurrence
	{
		public static readonly IfcDimensionCurveTerminator WR61 = new IfcDimensionCurveTerminator();
		protected IfcDimensionCurveTerminator() {}
	}
}
