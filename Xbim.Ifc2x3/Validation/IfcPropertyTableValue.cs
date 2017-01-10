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
namespace Xbim.Ifc2x3.PropertyResource
{
	public partial class IfcPropertyTableValue : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPropertyTableValue clause) {
			var retVal = false;
			if (clause == Where.IfcPropertyTableValue.WR1) {
				try {
					retVal = SIZEOF(DefiningValues) == SIZEOF(DefinedValues);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PropertyResource.IfcPropertyTableValue");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertyTableValue.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertyTableValue.WR2) {
				try {
					retVal = SIZEOF(this.DefiningValues.Where(temp => TYPEOF(temp) != TYPEOF(this.DefiningValues.ItemAt(0)))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PropertyResource.IfcPropertyTableValue");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertyTableValue.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertyTableValue.WR3) {
				try {
					retVal = SIZEOF(this.DefinedValues.Where(temp => TYPEOF(temp) != TYPEOF(this.DefinedValues.ItemAt(0)))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PropertyResource.IfcPropertyTableValue");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertyTableValue.WR3' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertyTableValue.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertyTableValue.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertyTableValue.WR3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyTableValue.WR3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcPropertyTableValue
	{
		public static readonly IfcPropertyTableValue WR1 = new IfcPropertyTableValue();
		public static readonly IfcPropertyTableValue WR2 = new IfcPropertyTableValue();
		public static readonly IfcPropertyTableValue WR3 = new IfcPropertyTableValue();
		protected IfcPropertyTableValue() {}
	}
}
