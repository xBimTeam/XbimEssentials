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
	public partial class IfcPropertyListValue : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPropertyListValue clause) {
			var retVal = false;
			if (clause == Where.IfcPropertyListValue.WR31) {
				try {
					retVal = SIZEOF(this.ListValues.Where(temp => !(TYPEOF(this.ListValues.ItemAt(0)) == TYPEOF(temp)))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PropertyResource.IfcPropertyListValue");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertyListValue.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertyListValue.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyListValue.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPropertyListValue
	{
		public static readonly IfcPropertyListValue WR31 = new IfcPropertyListValue();
		protected IfcPropertyListValue() {}
	}
}
