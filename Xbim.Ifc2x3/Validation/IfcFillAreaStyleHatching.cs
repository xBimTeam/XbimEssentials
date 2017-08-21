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
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial class IfcFillAreaStyleHatching : IExpressValidatable
	{
		public enum IfcFillAreaStyleHatchingClause
		{
			WR21,
			WR22,
			WR23,
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
					case IfcFillAreaStyleHatchingClause.WR21:
						retVal = !(Functions.TYPEOF(StartOfNextHatchLine).Contains("IFC2X3.IFCTWODIRECTIONREPEATFACTOR"));
						break;
					case IfcFillAreaStyleHatchingClause.WR22:
						retVal = !(Functions.EXISTS(PatternStart)) || (PatternStart.Dim == 2);
						break;
					case IfcFillAreaStyleHatchingClause.WR23:
						retVal = !(Functions.EXISTS(PointOfReferenceHatchLine)) || (PointOfReferenceHatchLine.Dim == 2);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationAppearanceResource.IfcFillAreaStyleHatching>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcFillAreaStyleHatching.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcFillAreaStyleHatchingClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFillAreaStyleHatchingClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFillAreaStyleHatchingClause.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.WR23", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
