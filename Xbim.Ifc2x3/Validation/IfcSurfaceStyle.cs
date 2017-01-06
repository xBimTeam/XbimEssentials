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
	public partial class IfcSurfaceStyle : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationAppearanceResource.IfcSurfaceStyle");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSurfaceStyle clause) {
			var retVal = false;
			if (clause == Where.IfcSurfaceStyle.WR11) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC2X3.IFCSURFACESTYLESHADING"))) <= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceStyle.WR11' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceStyle.WR12) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC2X3.IFCSURFACESTYLELIGHTING"))) <= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceStyle.WR12' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceStyle.WR13) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC2X3.IFCSURFACESTYLEREFRACTION"))) <= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceStyle.WR13' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceStyle.WR14) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC2X3.IFCSURFACESTYLEWITHTEXTURES"))) <= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceStyle.WR14' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceStyle.WR15) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC2X3.IFCEXTERNALLYDEFINEDSURFACESTYLE"))) <= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceStyle.WR15' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSurfaceStyle.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceStyle.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR12", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceStyle.WR13))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR13", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceStyle.WR14))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR14", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceStyle.WR15))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR15", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcSurfaceStyle
	{
		public static readonly IfcSurfaceStyle WR11 = new IfcSurfaceStyle();
		public static readonly IfcSurfaceStyle WR12 = new IfcSurfaceStyle();
		public static readonly IfcSurfaceStyle WR13 = new IfcSurfaceStyle();
		public static readonly IfcSurfaceStyle WR14 = new IfcSurfaceStyle();
		public static readonly IfcSurfaceStyle WR15 = new IfcSurfaceStyle();
		protected IfcSurfaceStyle() {}
	}
}
