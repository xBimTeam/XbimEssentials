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
namespace Xbim.Ifc4.StructuralElementsDomain
{
	public partial class IfcReinforcingBar : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcReinforcingBar clause) {
			var retVal = false;
			if (clause == Where.IfcReinforcingBar.CorrectPredefinedType) {
				try {
					retVal = !EXISTS(PredefinedType) || (PredefinedType != IfcReinforcingBarTypeEnum.USERDEFINED) || ((PredefinedType == IfcReinforcingBarTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralElementsDomain.IfcReinforcingBar");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcReinforcingBar.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcReinforcingBar.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCREINFORCINGBARTYPE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralElementsDomain.IfcReinforcingBar");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcReinforcingBar.CorrectTypeAssigned' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcReinforcingBar.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcReinforcingBar.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcReinforcingBar.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcReinforcingBar.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcReinforcingBar : IfcProduct
	{
		public static readonly IfcReinforcingBar CorrectPredefinedType = new IfcReinforcingBar();
		public static readonly IfcReinforcingBar CorrectTypeAssigned = new IfcReinforcingBar();
		protected IfcReinforcingBar() {}
	}
}
