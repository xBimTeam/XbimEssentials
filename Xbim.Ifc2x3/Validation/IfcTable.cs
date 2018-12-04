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
namespace Xbim.Ifc2x3.UtilityResource
{
	public partial class IfcTable : IExpressValidatable
	{
		public enum IfcTableClause
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
		public bool ValidateClause(IfcTableClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTableClause.WR1:
						retVal = Functions.SIZEOF(Rows.Where(Temp => Functions.HIINDEX(Temp.RowCells) != Functions.HIINDEX(Rows.ItemAt(0).RowCells))) == 0;
						break;
					case IfcTableClause.WR2:
						retVal = Functions.SIZEOF(Rows.Where(Temp => Functions.HIINDEX(Temp.RowCells) != Functions.HIINDEX(Rows.ItemAt(0).RowCells))) == 0;
						break;
					case IfcTableClause.WR3:
						retVal = ((0 <= NumberOfHeadings) && (NumberOfHeadings <= 1) );
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.UtilityResource.IfcTable>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTable.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTableClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTable.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTableClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTable.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTableClause.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTable.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
