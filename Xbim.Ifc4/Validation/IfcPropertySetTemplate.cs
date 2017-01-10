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
	public partial class IfcPropertySetTemplate : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPropertySetTemplate clause) {
			var retVal = false;
			if (clause == Where.IfcPropertySetTemplate.ExistsName) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcPropertySetTemplate");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertySetTemplate.ExistsName' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPropertySetTemplate.UniquePropertyNames) {
				try {
					retVal = IfcUniquePropertyTemplateNames(HasPropertyTemplates);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcPropertySetTemplate");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPropertySetTemplate.UniquePropertyNames' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertySetTemplate.ExistsName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySetTemplate.ExistsName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPropertySetTemplate.UniquePropertyNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertySetTemplate.UniquePropertyNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPropertySetTemplate
	{
		public static readonly IfcPropertySetTemplate ExistsName = new IfcPropertySetTemplate();
		public static readonly IfcPropertySetTemplate UniquePropertyNames = new IfcPropertySetTemplate();
		protected IfcPropertySetTemplate() {}
	}
}
