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
	public partial class IfcPreDefinedDimensionSymbol : IExpressValidatable
	{
		public enum IfcPreDefinedDimensionSymbolClause
		{
			WR31,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPreDefinedDimensionSymbolClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPreDefinedDimensionSymbolClause.WR31:
						retVal = Functions.NewArray("arc length", "conical taper", "counterbore", "countersink", "depth", "diameter", "plus minus", "radius", "slope", "spherical diameter", "spherical radius", "square").Contains(this/* as IfcPreDefinedItem*/.Name);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationDimensioningResource.IfcPreDefinedDimensionSymbol>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPreDefinedDimensionSymbol.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPreDefinedDimensionSymbolClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPreDefinedDimensionSymbol.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
