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
namespace Xbim.Ifc2x3.SharedBldgElements
{
	public partial class IfcWindowLiningProperties : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.SharedBldgElements.IfcWindowLiningProperties");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcWindowLiningProperties clause) {
			var retVal = false;
			if (clause == Where.IfcWindowLiningProperties.WR31) {
				try {
					retVal = !(!(EXISTS(LiningDepth)) && EXISTS(LiningThickness));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcWindowLiningProperties.WR31' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcWindowLiningProperties.WR32) {
				try {
					retVal = !(!(EXISTS(FirstTransomOffset)) && EXISTS(SecondTransomOffset));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcWindowLiningProperties.WR32' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcWindowLiningProperties.WR33) {
				try {
					retVal = !(!(EXISTS(FirstMullionOffset)) && EXISTS(SecondMullionOffset));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcWindowLiningProperties.WR33' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcWindowLiningProperties.WR34) {
				try {
					retVal = EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0]) && (TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ToArray()[0]).Contains("IFC2X3.IFCWINDOWSTYLE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcWindowLiningProperties.WR34' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcWindowLiningProperties.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowLiningProperties.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcWindowLiningProperties.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowLiningProperties.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcWindowLiningProperties.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowLiningProperties.WR33", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcWindowLiningProperties.WR34))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowLiningProperties.WR34", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcWindowLiningProperties
	{
		public static readonly IfcWindowLiningProperties WR31 = new IfcWindowLiningProperties();
		public static readonly IfcWindowLiningProperties WR32 = new IfcWindowLiningProperties();
		public static readonly IfcWindowLiningProperties WR33 = new IfcWindowLiningProperties();
		public static readonly IfcWindowLiningProperties WR34 = new IfcWindowLiningProperties();
		protected IfcWindowLiningProperties() {}
	}
}
