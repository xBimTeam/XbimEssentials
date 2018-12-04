using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
	public partial class IfcDimensionCurveDirectedCallout : IExpressValidatable
	{
		public enum IfcDimensionCurveDirectedCalloutClause
		{
			WR41,
			WR42,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDimensionCurveDirectedCalloutClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDimensionCurveDirectedCalloutClause.WR41:
						retVal = Functions.SIZEOF(this/* as IfcDraughtingCallout*/.Contents.Where(Dc => (Functions.TYPEOF(Dc).Contains("IFC2X3.IFCDIMENSIONCURVE")))) == 1;
						break;
					case IfcDimensionCurveDirectedCalloutClause.WR42:
						retVal = Functions.SIZEOF(this.Contents.Where(Dc => (Functions.TYPEOF(Dc).Contains("IFC2X3.IFCPROJECTIONCURVE")))) <= 2;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCurveDirectedCallout>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCurveDirectedCallout.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDimensionCurveDirectedCalloutClause.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurveDirectedCallout.WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDimensionCurveDirectedCalloutClause.WR42))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCurveDirectedCallout.WR42", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
