using System;
using log4net;
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
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.UtilityResource
{
	public partial class IfcTable : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTable clause) {
			var retVal = false;
			if (clause == Where.IfcTable.WR1) {
				try {
					retVal = SIZEOF(Rows.Where(Temp => HIINDEX(Temp.RowCells) != HIINDEX(Rows.ItemAt(0).RowCells))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.UtilityResource.IfcTable");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTable.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTable.WR2) {
				try {
					retVal = SIZEOF(Rows.Where(Temp => HIINDEX(Temp.RowCells) != HIINDEX(Rows.ItemAt(0).RowCells))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.UtilityResource.IfcTable");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTable.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTable.WR3) {
				try {
					retVal = ((0 <= NumberOfHeadings) && (NumberOfHeadings <= 1) );
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.UtilityResource.IfcTable");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTable.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTable.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTable.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTable.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTable.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTable.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTable.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcTable
	{
		public static readonly IfcTable WR1 = new IfcTable();
		public static readonly IfcTable WR2 = new IfcTable();
		public static readonly IfcTable WR3 = new IfcTable();
		protected IfcTable() {}
	}
}
