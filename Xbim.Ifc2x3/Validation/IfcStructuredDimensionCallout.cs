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
	public partial class IfcStructuredDimensionCallout : IExpressValidatable
	{
		public enum IfcStructuredDimensionCalloutClause
		{
			WR31,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuredDimensionCalloutClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuredDimensionCalloutClause.WR31:
						retVal = Functions.SIZEOF(this.Contents.Where(Con => (Functions.TYPEOF(Con).Contains("IFC2X3.IFCANNOTATIONTEXTOCCURRENCE"))).Where(Ato => (!(Functions.NewArray("dimension value", "tolerance value", "unit text", "prefix text", "suffix text").Contains(Ato.AsIfcAnnotationTextOccurrence().Name))))) == 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationDimensioningResource.IfcStructuredDimensionCallout>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStructuredDimensionCallout.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcStructuredDimensionCalloutClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuredDimensionCallout.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
