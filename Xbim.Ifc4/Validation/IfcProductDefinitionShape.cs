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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcProductDefinitionShape : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcProductDefinitionShape");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcProductDefinitionShape clause) {
			var retVal = false;
			if (clause == Where.IfcProductDefinitionShape.OnlyShapeModel) {
				try {
					retVal = SIZEOF(Representations.Where(temp => (!(TYPEOF(temp).Contains("IFC4.IFCSHAPEMODEL"))))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcProductDefinitionShape.OnlyShapeModel' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcProductDefinitionShape.OnlyShapeModel))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProductDefinitionShape.OnlyShapeModel", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcProductDefinitionShape
	{
		public static readonly IfcProductDefinitionShape OnlyShapeModel = new IfcProductDefinitionShape();
		protected IfcProductDefinitionShape() {}
	}
}
