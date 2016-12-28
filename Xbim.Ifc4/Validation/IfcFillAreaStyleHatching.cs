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
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial class IfcFillAreaStyleHatching : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcFillAreaStyleHatching");

		/// <summary>
		/// Tests the express where clause PatternStart2D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool PatternStart2D() {
			var retVal = false;
			try {
				retVal = !(EXISTS(PatternStart)) || (PatternStart.Dim == 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'PatternStart2D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause RefHatchLine2D
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool RefHatchLine2D() {
			var retVal = false;
			try {
				retVal = !(EXISTS(PointOfReferenceHatchLine)) || (PointOfReferenceHatchLine.Dim == 2);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'RefHatchLine2D' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!PatternStart2D())
				yield return new ValidationResult() { Item = this, IssueSource = "PatternStart2D", IssueType = ValidationFlags.EntityWhereClauses };
			if (!RefHatchLine2D())
				yield return new ValidationResult() { Item = this, IssueSource = "RefHatchLine2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
