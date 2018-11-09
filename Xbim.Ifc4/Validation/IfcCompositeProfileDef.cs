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
	public partial class IfcCompositeProfileDef : IExpressValidatable
	{
		public enum IfcCompositeProfileDefClause
		{
			InvariantProfileType,
			NoRecursion,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCompositeProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCompositeProfileDefClause.InvariantProfileType:
						retVal = Functions.SIZEOF(Profiles.Where(temp => temp.ProfileType != Profiles.ItemAt(0).ProfileType)) == 0;
						break;
					case IfcCompositeProfileDefClause.NoRecursion:
						retVal = Functions.SIZEOF(Profiles.Where(temp => Functions.TYPEOF(temp).Contains("IFC4.IFCCOMPOSITEPROFILEDEF"))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProfileResource.IfcCompositeProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCompositeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCompositeProfileDefClause.InvariantProfileType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeProfileDef.InvariantProfileType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcCompositeProfileDefClause.NoRecursion))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCompositeProfileDef.NoRecursion", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
