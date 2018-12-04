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
						retVal = Functions.SIZEOF(this.FillStyles.Where(Style => Functions.TYPEOF(Style).Contains("IFC4.IFCCOLOUR"))) <= 1;
						break;
					case IfcFillAreaStyleClause.MaxOneExtHatchStyle:
						retVal = Functions.SIZEOF(this.FillStyles.Where(Style => Functions.TYPEOF(Style).Contains("IFC4.IFCEXTERNALLYDEFINEDHATCHSTYLE"))) <= 1;
						break;
					case IfcFillAreaStyleClause.ConsistentHatchStyleDef:
						retVal = Functions.IfcCorrectFillAreaStyle(this.FillStyles);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PresentationAppearanceResource.IfcFillAreaStyle>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcFillAreaStyle.{0}' for #{1}.", clause,EntityLabel), ex);
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
