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
	public partial class IfcCircleHollowProfileDef : IExpressValidatable
	{
		public enum IfcCircleHollowProfileDefClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCircleHollowProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCircleHollowProfileDefClause.WR1:
						retVal = WallThickness < this/* as IfcCircleProfileDef*/.Radius;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProfileResource.IfcCircleHollowProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCircleHollowProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCircleHollowProfileDefClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCircleHollowProfileDef.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
