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
	public partial class IfcPropertyBoundedValue : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPropertyBoundedValue clause) {
			var retVal = false;
			if (clause == Where.IfcPropertyBoundedValue.WR21) {
				try {
					retVal = !(EXISTS(UpperBoundValue)) || !(EXISTS(LowerBoundValue)) || (TYPEOF(UpperBoundValue) == TYPEOF(LowerBoundValue));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PropertyResource.IfcPropertyBoundedValue");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertyBoundedValue.WR21' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertyBoundedValue.WR22) {
				try {
					retVal = EXISTS(UpperBoundValue) || EXISTS(LowerBoundValue);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PropertyResource.IfcPropertyBoundedValue");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertyBoundedValue.WR22' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertyBoundedValue.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyBoundedValue.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertyBoundedValue.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyBoundedValue.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcPropertyBoundedValue
	{
		public static readonly IfcPropertyBoundedValue WR21 = new IfcPropertyBoundedValue();
		public static readonly IfcPropertyBoundedValue WR22 = new IfcPropertyBoundedValue();
		protected IfcPropertyBoundedValue() {}
	}
}
