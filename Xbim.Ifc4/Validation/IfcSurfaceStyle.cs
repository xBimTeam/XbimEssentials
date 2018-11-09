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
	public partial class IfcSurfaceStyle : IExpressValidatable
	{
		public enum IfcSurfaceStyleClause
		{
			MaxOneShading,
			MaxOneLighting,
			MaxOneRefraction,
			MaxOneTextures,
			MaxOneExtDefined,
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
					case IfcSurfaceStyleClause.MaxOneShading:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLESHADING"))) <= 1;
						break;
					case IfcSurfaceStyleClause.MaxOneLighting:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLELIGHTING"))) <= 1;
						break;
					case IfcSurfaceStyleClause.MaxOneRefraction:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLEREFRACTION"))) <= 1;
						break;
					case IfcSurfaceStyleClause.MaxOneTextures:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLEWITHTEXTURES"))) <= 1;
						break;
					case IfcSurfaceStyleClause.MaxOneExtDefined:
						retVal = Functions.SIZEOF(this.Styles.Where(Style => Functions.TYPEOF(Style).Contains("IFC4.IFCEXTERNALLYDEFINEDSURFACESTYLE"))) <= 1;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PresentationAppearanceResource.IfcSurfaceStyle>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceStyle.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcSurfaceStyleClause.MaxOneShading))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneShading", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceStyleClause.MaxOneLighting))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneLighting", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceStyleClause.MaxOneRefraction))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneRefraction", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceStyleClause.MaxOneTextures))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneTextures", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSurfaceStyleClause.MaxOneExtDefined))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneExtDefined", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
