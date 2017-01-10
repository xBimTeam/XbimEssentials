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
namespace Xbim.Ifc2x3.Kernel
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
			if (clause == Where.IfcPropertySet.WR31) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.Kernel.IfcPropertySet");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertySet.WR31' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertySet.WR32) {
				try {
					retVal = IfcUniquePropertyName(HasProperties);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.Kernel.IfcPropertySet");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertySet.WR32' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertySet.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySet.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertySet.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySet.WR32", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcPropertySet
	{
		public static readonly IfcPropertySet WR31 = new IfcPropertySet();
		public static readonly IfcPropertySet WR32 = new IfcPropertySet();
		protected IfcPropertySet() {}
	}
}
