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
namespace Xbim.Ifc2x3.RepresentationResource
{
	public partial class IfcProductDefinitionShape : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcProductDefinitionShape clause) {
			var retVal = false;
			if (clause == Where.IfcProductDefinitionShape.WR11) {
				try {
					retVal = SIZEOF(Representations.Where(temp => (!(TYPEOF(temp).Contains("IFC2X3.IFCSHAPEMODEL"))))) == 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.RepresentationResource.IfcProductDefinitionShape");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProductDefinitionShape.WR11' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcProductDefinitionShape.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProductDefinitionShape.WR11", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcProductDefinitionShape
	{
		public static readonly IfcProductDefinitionShape WR11 = new IfcProductDefinitionShape();
		protected IfcProductDefinitionShape() {}
	}
}
