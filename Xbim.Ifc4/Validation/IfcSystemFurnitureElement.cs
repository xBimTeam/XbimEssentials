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
namespace Xbim.Ifc4.SharedFacilitiesElements
{
	public partial class IfcSystemFurnitureElement : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedFacilitiesElements.IfcSystemFurnitureElement");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSystemFurnitureElement clause) {
			var retVal = false;
			if (clause == Where.IfcSystemFurnitureElement.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcSystemFurnitureElementTypeEnum.USERDEFINED) || ((PredefinedType == IfcSystemFurnitureElementTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSystemFurnitureElement.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSystemFurnitureElement.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCSYSTEMFURNITUREELEMENTTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSystemFurnitureElement.CorrectTypeAssigned' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcSystemFurnitureElement.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSystemFurnitureElement.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSystemFurnitureElement.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSystemFurnitureElement.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSystemFurnitureElement : IfcProduct
	{
		public static readonly IfcSystemFurnitureElement CorrectPredefinedType = new IfcSystemFurnitureElement();
		public static readonly IfcSystemFurnitureElement CorrectTypeAssigned = new IfcSystemFurnitureElement();
		protected IfcSystemFurnitureElement() {}
	}
}
