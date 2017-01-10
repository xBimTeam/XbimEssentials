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
	public partial class IfcProduct : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcProduct clause) {
			var retVal = false;
			if (clause == Where.IfcProduct.PlacementForShapeRepresentation) {
				try {
					retVal = (EXISTS(Representation) && EXISTS(ObjectPlacement)) || (EXISTS(Representation) && (SIZEOF(Representation.Representations.Where(temp => TYPEOF(temp).Contains("IFC4.IFCSHAPEREPRESENTATION"))) == 0)) || (!(EXISTS(Representation)));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcProduct");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProduct.PlacementForShapeRepresentation' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcProduct.PlacementForShapeRepresentation))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProduct.PlacementForShapeRepresentation", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcProduct : IfcObject
	{
		public static readonly IfcProduct PlacementForShapeRepresentation = new IfcProduct();
		protected IfcProduct() {}
	}
}
