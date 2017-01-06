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
	public partial class IfcFillAreaStyle : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcFillAreaStyle");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcFillAreaStyle clause) {
			var retVal = false;
			if (clause == Where.IfcFillAreaStyle.MaxOneColour) {
				try {
					retVal = SIZEOF(this.FillStyles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCCOLOUR"))) <= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFillAreaStyle.MaxOneColour' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcFillAreaStyle.MaxOneExtHatchStyle) {
				try {
					retVal = SIZEOF(this.FillStyles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCEXTERNALLYDEFINEDHATCHSTYLE"))) <= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFillAreaStyle.MaxOneExtHatchStyle' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcFillAreaStyle.ConsistentHatchStyleDef) {
				try {
					retVal = IfcCorrectFillAreaStyle(this.FillStyles);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcFillAreaStyle.ConsistentHatchStyleDef' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcFillAreaStyle.MaxOneColour))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.MaxOneColour", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcFillAreaStyle.MaxOneExtHatchStyle))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.MaxOneExtHatchStyle", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcFillAreaStyle.ConsistentHatchStyleDef))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.ConsistentHatchStyleDef", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcFillAreaStyle
	{
		public static readonly IfcFillAreaStyle MaxOneColour = new IfcFillAreaStyle();
		public static readonly IfcFillAreaStyle MaxOneExtHatchStyle = new IfcFillAreaStyle();
		public static readonly IfcFillAreaStyle ConsistentHatchStyleDef = new IfcFillAreaStyle();
		protected IfcFillAreaStyle() {}
	}
}
