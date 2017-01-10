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
	public partial class IfcTypeObject : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTypeObject clause) {
			var retVal = false;
			if (clause == Where.IfcTypeObject.NameRequired) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcTypeObject");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTypeObject.NameRequired' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTypeObject.UniquePropertySetNames) {
				try {
					retVal = (!(EXISTS(HasPropertySets))) || IfcUniquePropertySetNames(HasPropertySets);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcTypeObject");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTypeObject.UniquePropertySetNames' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTypeObject.NameRequired))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTypeObject.NameRequired", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTypeObject.UniquePropertySetNames))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTypeObject.UniquePropertySetNames", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcTypeObject
	{
		public static readonly IfcTypeObject NameRequired = new IfcTypeObject();
		public static readonly IfcTypeObject UniquePropertySetNames = new IfcTypeObject();
		protected IfcTypeObject() {}
	}
}
