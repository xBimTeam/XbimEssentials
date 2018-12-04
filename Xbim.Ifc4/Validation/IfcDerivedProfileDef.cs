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
	public partial class IfcDerivedProfileDef : IExpressValidatable
	{
		public enum IfcDerivedProfileDefClause
		{
			InvariantProfileType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDerivedProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDerivedProfileDefClause.InvariantProfileType:
						retVal = this/* as IfcProfileDef*/.ProfileType == ParentProfile.ProfileType;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProfileResource.IfcDerivedProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcDerivedProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDerivedProfileDefClause.InvariantProfileType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDerivedProfileDef.InvariantProfileType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
