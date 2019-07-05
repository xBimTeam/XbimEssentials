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
	public partial class IfcSurfaceStyle : IExpressValidatable
	{
		public enum IfcSurfaceStyleClause
		{
			WR11,
			WR12,
			WR13,
			WR14,
			WR15,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSurfaceStyleClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSurfaceStyleClause.WR11:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC2X3.IFCSURFACESTYLESHADING"))) <= 1;
						break;
					case IfcSurfaceStyleClause.WR12:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC2X3.IFCSURFACESTYLELIGHTING"))) <= 1;
						break;
					case IfcSurfaceStyleClause.WR13:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC2X3.IFCSURFACESTYLEREFRACTION"))) <= 1;
						break;
					case IfcSurfaceStyleClause.WR14:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC2X3.IFCSURFACESTYLEWITHTEXTURES"))) <= 1;
						break;
					case IfcSurfaceStyleClause.WR15:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC2X3.IFCEXTERNALLYDEFINEDSURFACESTYLE"))) <= 1;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationAppearanceResource.IfcSurfaceStyle>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceStyle.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcSurfaceStyleClause.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceStyleClause.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR12", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceStyleClause.WR13))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR13", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceStyleClause.WR14))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR14", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceStyleClause.WR15))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.WR15", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
