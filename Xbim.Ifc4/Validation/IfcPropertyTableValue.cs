using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.PropertyResource
{
	public partial class IfcPropertyTableValue : IExpressValidatable
	{
		public enum IfcPropertyTableValueClause
		{
			WR21,
			WR22,
			WR23,
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
					case IfcPropertyTableValueClause.WR21:
						retVal = (!(EXISTS(DefiningValues)) && !(EXISTS(DefinedValues))) || (SIZEOF(DefiningValues) == SIZEOF(DefinedValues));
						break;
					case IfcPropertyTableValueClause.WR22:
						retVal = !(EXISTS(DefiningValues)) || (SIZEOF(this.DefiningValues.Where(temp => TYPEOF(temp) != TYPEOF(this.DefiningValues.ItemAt(0)))) == 0);
						break;
					case IfcPropertyTableValueClause.WR23:
						retVal = !(EXISTS(DefinedValues)) || (SIZEOF(this.DefinedValues.Where(temp => TYPEOF(temp) != TYPEOF(this.DefinedValues.ItemAt(0)))) == 0);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.PropertyResource.IfcPropertyTableValue");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertyTableValue.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPropertyTableValueClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPropertyTableValueClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcPropertyTableValueClause.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR23", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
