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
namespace Xbim.Ifc4.Kernel
{
	public partial class IfcPropertySet : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPropertySet clause) {
			var retVal = false;
			if (clause == Where.IfcPropertySet.ExistsName) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcPropertySet");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertySet.ExistsName' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertySet.UniquePropertyNames) {
				try {
					retVal = IfcUniquePropertyName(HasProperties);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcPropertySet");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertySet.UniquePropertyNames' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertySet.ExistsName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySet.ExistsName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertySet.UniquePropertyNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySet.UniquePropertyNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPropertySet
	{
		public static readonly IfcPropertySet ExistsName = new IfcPropertySet();
		public static readonly IfcPropertySet UniquePropertyNames = new IfcPropertySet();
		protected IfcPropertySet() {}
	}
}
