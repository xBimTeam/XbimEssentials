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
	public partial class IfcZShapeProfileDef : IExpressValidatable
	{
		public enum IfcZShapeProfileDefClause
		{
			ValidFlangeThickness,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcZShapeProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcZShapeProfileDefClause.ValidFlangeThickness:
						retVal = FlangeThickness < (Depth / 2);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProfileResource.IfcZShapeProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcZShapeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcZShapeProfileDefClause.ValidFlangeThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcZShapeProfileDef.ValidFlangeThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
