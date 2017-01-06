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
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial class IfcFillAreaStyleHatching : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationAppearanceResource.IfcFillAreaStyleHatching");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcFillAreaStyleHatching clause) {
			var retVal = false;
			if (clause == Where.IfcFillAreaStyleHatching.WR21) {
				try {
					retVal = !(TYPEOF(StartOfNextHatchLine).Contains("IFC2X3.IFCTWODIRECTIONREPEATFACTOR"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFillAreaStyleHatching.WR21' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcFillAreaStyleHatching.WR22) {
				try {
					retVal = !(EXISTS(PatternStart)) || (PatternStart.Dim == 2);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFillAreaStyleHatching.WR22' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcFillAreaStyleHatching.WR23) {
				try {
					retVal = !(EXISTS(PointOfReferenceHatchLine)) || (PointOfReferenceHatchLine.Dim == 2);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFillAreaStyleHatching.WR23' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcFillAreaStyleHatching.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcFillAreaStyleHatching.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcFillAreaStyleHatching.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyleHatching.WR23", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcFillAreaStyleHatching
	{
		public static readonly IfcFillAreaStyleHatching WR21 = new IfcFillAreaStyleHatching();
		public static readonly IfcFillAreaStyleHatching WR22 = new IfcFillAreaStyleHatching();
		public static readonly IfcFillAreaStyleHatching WR23 = new IfcFillAreaStyleHatching();
		protected IfcFillAreaStyleHatching() {}
	}
}
