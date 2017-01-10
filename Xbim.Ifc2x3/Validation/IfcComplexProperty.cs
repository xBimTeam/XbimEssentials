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
	public partial class IfcComplexProperty : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcComplexProperty clause) {
			var retVal = false;
			if (clause == Where.IfcComplexProperty.WR21) {
				try {
					retVal = SIZEOF(HasProperties.Where(temp => Object.ReferenceEquals(this, temp))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PropertyResource.IfcComplexProperty");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcComplexProperty.WR21' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcComplexProperty.WR22) {
				try {
					retVal = IfcUniquePropertyName(HasProperties);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PropertyResource.IfcComplexProperty");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcComplexProperty.WR22' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcComplexProperty.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcComplexProperty.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcComplexProperty.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcComplexProperty.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcComplexProperty
	{
		public static readonly IfcComplexProperty WR21 = new IfcComplexProperty();
		public static readonly IfcComplexProperty WR22 = new IfcComplexProperty();
		protected IfcComplexProperty() {}
	}
}
