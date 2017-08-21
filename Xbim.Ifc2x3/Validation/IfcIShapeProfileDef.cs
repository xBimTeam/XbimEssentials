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
namespace Xbim.Ifc2x3.ProfileResource
{
	public partial class IfcIShapeProfileDef : IExpressValidatable
	{
		public enum IfcIShapeProfileDefClause
		{
			WR1,
			WR2,
			WR3,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcIShapeProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcIShapeProfileDefClause.WR1:
						retVal = FlangeThickness < (OverallDepth / 2);
						break;
					case IfcIShapeProfileDefClause.WR2:
						retVal = WebThickness < OverallWidth;
						break;
					case IfcIShapeProfileDefClause.WR3:
						retVal = !(Functions.EXISTS(FilletRadius)) || ((FilletRadius <= (OverallWidth - WebThickness) / 2) && (FilletRadius <= (OverallDepth - (2 * FlangeThickness)) / 2));
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProfileResource.IfcIShapeProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcIShapeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcIShapeProfileDefClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcIShapeProfileDefClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcIShapeProfileDefClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
