using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial class IfcFillAreaStyleHatching : IExpressValidatable
	{
		public enum IfcFillAreaStyleHatchingClause
		{
			PatternStart2D,
			RefHatchLine2D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFillAreaStyleHatchingClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFillAreaStyleHatchingClause.PatternStart2D:
						retVal = !(Functions.EXISTS(PatternStart)) || (PatternStart.Dim == 2);
						break;
					case IfcFillAreaStyleHatchingClause.RefHatchLine2D:
						retVal = !(Functions.EXISTS(PointOfReferenceHatchLine)) || (PointOfReferenceHatchLine.Dim == 2);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PresentationAppearanceResource.IfcFillAreaStyleHatching>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcFillAreaStyleHatching.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcFillAreaStyleHatchingClause.PatternStart2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.PatternStart2D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFillAreaStyleHatchingClause.RefHatchLine2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.RefHatchLine2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
