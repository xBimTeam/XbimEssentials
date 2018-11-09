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
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcIShapeProfileDef : IExpressValidatable
	{
		public enum IfcIShapeProfileDefClause
		{
			ValidFlangeThickness,
			ValidWebThickness,
			ValidFilletRadius,
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
					case IfcIShapeProfileDefClause.ValidFlangeThickness:
						retVal = (2 * FlangeThickness) < OverallDepth;
						break;
					case IfcIShapeProfileDefClause.ValidWebThickness:
						retVal = WebThickness < OverallWidth;
						break;
					case IfcIShapeProfileDefClause.ValidFilletRadius:
						retVal = !(Functions.EXISTS(FilletRadius)) || ((FilletRadius <= (OverallWidth - WebThickness) / 2) && (FilletRadius <= (OverallDepth - (2 * FlangeThickness)) / 2));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProfileResource.IfcIShapeProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcIShapeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcIShapeProfileDefClause.ValidFlangeThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcIShapeProfileDefClause.ValidWebThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.ValidWebThickness", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcIShapeProfileDefClause.ValidFilletRadius))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcIShapeProfileDef.ValidFilletRadius", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
