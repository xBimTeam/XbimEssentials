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
	public partial class IfcComplexPropertyTemplate : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcComplexPropertyTemplate clause) {
			var retVal = false;
			if (clause == Where.IfcComplexPropertyTemplate.UniquePropertyNames) {
				try {
					retVal = IfcUniquePropertyTemplateNames(HasPropertyTemplates);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcComplexPropertyTemplate");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcComplexPropertyTemplate.UniquePropertyNames' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcComplexPropertyTemplate.NoSelfReference) {
				try {
					retVal = SIZEOF(HasPropertyTemplates.Where(temp => Object.ReferenceEquals(this, temp))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcComplexPropertyTemplate");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcComplexPropertyTemplate.NoSelfReference' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcComplexPropertyTemplate.UniquePropertyNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcComplexPropertyTemplate.UniquePropertyNames", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcComplexPropertyTemplate.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcComplexPropertyTemplate.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcComplexPropertyTemplate
	{
		public static readonly IfcComplexPropertyTemplate UniquePropertyNames = new IfcComplexPropertyTemplate();
		public static readonly IfcComplexPropertyTemplate NoSelfReference = new IfcComplexPropertyTemplate();
		protected IfcComplexPropertyTemplate() {}
	}
}
