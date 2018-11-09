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
	public partial class IfcRoundedRectangleProfileDef : IExpressValidatable
	{
		public enum IfcRoundedRectangleProfileDefClause
		{
			WR31,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRoundedRectangleProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRoundedRectangleProfileDefClause.WR31:
						retVal = ((RoundingRadius <= (this/* as IfcRectangleProfileDef*/.XDim / 2)) && (RoundingRadius <= (this/* as IfcRectangleProfileDef*/.YDim / 2)));
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.ProfileResource.IfcRoundedRectangleProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRoundedRectangleProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRoundedRectangleProfileDefClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRoundedRectangleProfileDef.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
