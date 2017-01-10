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

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSurfaceStyle clause) {
			var retVal = false;
			if (clause == Where.IfcSurfaceStyle.MaxOneShading) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLESHADING"))) <= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcSurfaceStyle");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceStyle.MaxOneShading' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceStyle.MaxOneLighting) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLELIGHTING"))) <= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcSurfaceStyle");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceStyle.MaxOneLighting' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceStyle.MaxOneRefraction) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLEREFRACTION"))) <= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcSurfaceStyle");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceStyle.MaxOneRefraction' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceStyle.MaxOneTextures) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCSURFACESTYLEWITHTEXTURES"))) <= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcSurfaceStyle");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceStyle.MaxOneTextures' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceStyle.MaxOneExtDefined) {
				try {
					retVal = SIZEOF(this.Styles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCEXTERNALLYDEFINEDSURFACESTYLE"))) <= 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcSurfaceStyle");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSurfaceStyle.MaxOneExtDefined' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSurfaceStyle.MaxOneShading))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneShading", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceStyle.MaxOneLighting))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneLighting", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceStyle.MaxOneRefraction))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneRefraction", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceStyle.MaxOneTextures))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneTextures", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceStyle.MaxOneExtDefined))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceStyle.MaxOneExtDefined", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSurfaceStyle
	{
		public static readonly IfcSurfaceStyle MaxOneShading = new IfcSurfaceStyle();
		public static readonly IfcSurfaceStyle MaxOneLighting = new IfcSurfaceStyle();
		public static readonly IfcSurfaceStyle MaxOneRefraction = new IfcSurfaceStyle();
		public static readonly IfcSurfaceStyle MaxOneTextures = new IfcSurfaceStyle();
		public static readonly IfcSurfaceStyle MaxOneExtDefined = new IfcSurfaceStyle();
		protected IfcSurfaceStyle() {}
	}
}
