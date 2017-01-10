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
		public enum IfcFillAreaStyleClause
		{
			MaxOneColour,
			MaxOneExtHatchStyle,
			ConsistentHatchStyleDef,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFillAreaStyleClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFillAreaStyleClause.MaxOneColour:
						retVal = SIZEOF(this.FillStyles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCCOLOUR"))) <= 1;
						break;
					case IfcFillAreaStyleClause.MaxOneExtHatchStyle:
						retVal = SIZEOF(this.FillStyles.Where(Style => TYPEOF(Style).Contains("IFC4.IFCEXTERNALLYDEFINEDHATCHSTYLE"))) <= 1;
						break;
					case IfcFillAreaStyleClause.ConsistentHatchStyleDef:
						retVal = IfcCorrectFillAreaStyle(this.FillStyles);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcFillAreaStyle");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcFillAreaStyle.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcFillAreaStyleClause.MaxOneColour))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.MaxOneColour", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFillAreaStyleClause.MaxOneExtHatchStyle))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.MaxOneExtHatchStyle", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFillAreaStyleClause.ConsistentHatchStyleDef))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.ConsistentHatchStyleDef", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
