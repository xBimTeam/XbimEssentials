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
	public partial class IfcSurfaceStyle : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcSurfaceStyle");

		/// <summary>
		/// Tests the express where clause MaxOneShading
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MaxOneShading() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLESHADING"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MaxOneShading' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause MaxOneLighting
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MaxOneLighting() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLELIGHTING"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MaxOneLighting' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause MaxOneRefraction
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MaxOneRefraction() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLEREFRACTION"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MaxOneRefraction' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause MaxOneTextures
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MaxOneTextures() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLEWITHTEXTURES"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MaxOneTextures' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause MaxOneExtDefined
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MaxOneExtDefined() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCEXTERNALLYDEFINEDSURFACESTYLE"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MaxOneExtDefined' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!MaxOneShading())
				yield return new ValidationResult() { Item = this, IssueSource = "MaxOneShading", IssueType = ValidationFlags.EntityWhereClauses };
			if (!MaxOneLighting())
				yield return new ValidationResult() { Item = this, IssueSource = "MaxOneLighting", IssueType = ValidationFlags.EntityWhereClauses };
			if (!MaxOneRefraction())
				yield return new ValidationResult() { Item = this, IssueSource = "MaxOneRefraction", IssueType = ValidationFlags.EntityWhereClauses };
			if (!MaxOneTextures())
				yield return new ValidationResult() { Item = this, IssueSource = "MaxOneTextures", IssueType = ValidationFlags.EntityWhereClauses };
			if (!MaxOneExtDefined())
				yield return new ValidationResult() { Item = this, IssueSource = "MaxOneExtDefined", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
