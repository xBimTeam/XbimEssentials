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
namespace Xbim.Ifc2x3.PropertyResource
{
	public partial class IfcPropertyTableValue : IExpressValidatable
	{
		public enum IfcPropertyTableValueClause
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
		public bool ValidateClause(IfcPropertyTableValueClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPropertyTableValueClause.WR1:
						retVal = Functions.SIZEOF(DefiningValues) == Functions.SIZEOF(DefinedValues);
						break;
					case IfcPropertyTableValueClause.WR2:
						retVal = Functions.SIZEOF(this.DefiningValues.Where(temp => Functions.TYPEOF(temp) != Functions.TYPEOF(this.DefiningValues.ItemAt(0)))) == 0;
						break;
					case IfcPropertyTableValueClause.WR3:
						retVal = Functions.SIZEOF(this.DefinedValues.Where(temp => Functions.TYPEOF(temp) != Functions.TYPEOF(this.DefinedValues.ItemAt(0)))) == 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PropertyResource.IfcPropertyTableValue>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPropertyTableValue.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPropertyTableValueClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPropertyTableValueClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPropertyTableValueClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
