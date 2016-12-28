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
		/// Tests the express where clause MaxOneColour
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MaxOneColour() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.FillStyles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCCOLOUR"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MaxOneColour' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause MaxOneExtHatchStyle
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool MaxOneExtHatchStyle() {
			var retVal = false;
			try {
				retVal = SIZEOF(this.FillStyles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCEXTERNALLYDEFINEDHATCHSTYLE"))) <= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'MaxOneExtHatchStyle' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause ConsistentHatchStyleDef
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ConsistentHatchStyleDef() {
			var retVal = false;
			try {
				retVal = IfcCorrectFillAreaStyle(this.FillStyles);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ConsistentHatchStyleDef' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!MaxOneColour())
				yield return new ValidationResult() { Item = this, IssueSource = "MaxOneColour", IssueType = ValidationFlags.EntityWhereClauses };
			if (!MaxOneExtHatchStyle())
				yield return new ValidationResult() { Item = this, IssueSource = "MaxOneExtHatchStyle", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ConsistentHatchStyleDef())
				yield return new ValidationResult() { Item = this, IssueSource = "ConsistentHatchStyleDef", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
