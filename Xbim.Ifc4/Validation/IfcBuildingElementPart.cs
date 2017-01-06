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
namespace Xbim.Ifc4.SharedComponentElements
{
	public partial class IfcBuildingElementPart : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedComponentElements.IfcBuildingElementPart");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBuildingElementPart clause) {
			var retVal = false;
			if (clause == Where.IfcBuildingElementPart.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcBuildingElementPartTypeEnum.USERDEFINED) || ((PredefinedType == IfcBuildingElementPartTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBuildingElementPart.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBuildingElementPart.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCBUILDINGELEMENTPARTTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBuildingElementPart.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcBuildingElementPart.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElementPart.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBuildingElementPart.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBuildingElementPart.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcBuildingElementPart : IfcProduct
	{
		public static readonly IfcBuildingElementPart CorrectPredefinedType = new IfcBuildingElementPart();
		public static readonly IfcBuildingElementPart CorrectTypeAssigned = new IfcBuildingElementPart();
		protected IfcBuildingElementPart() {}
	}
}
