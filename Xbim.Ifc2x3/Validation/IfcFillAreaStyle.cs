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
	public partial class IfcFillAreaStyle : IExpressValidatable
	{
		public enum IfcFillAreaStyleClause
		{
			WR11,
			WR12,
			WR13,
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
					case IfcFillAreaStyleClause.WR11:
						retVal = Functions.SIZEOF(this.FillStyles.Where(Style => Functions.TYPEOF(Style).Contains("IFC2X3.IFCCOLOUR"))) <= 1;
						break;
					case IfcFillAreaStyleClause.WR12:
						retVal = Functions.SIZEOF(this.FillStyles.Where(Style => Functions.TYPEOF(Style).Contains("IFC2X3.IFCEXTERNALLYDEFINEDHATCHSTYLE"))) <= 1;
						break;
					case IfcFillAreaStyleClause.WR13:
						retVal = Functions.IfcCorrectFillAreaStyle(this.FillStyles);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationAppearanceResource.IfcFillAreaStyle>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcFillAreaStyle.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcFillAreaStyleClause.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFillAreaStyleClause.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.WR12", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFillAreaStyleClause.WR13))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFillAreaStyle.WR13", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
