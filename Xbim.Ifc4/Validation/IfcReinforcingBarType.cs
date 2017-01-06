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
	public partial class IfcReinforcingBarType : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralElementsDomain.IfcReinforcingBarType");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcReinforcingBarType clause) {
			var retVal = false;
			if (clause == Where.IfcReinforcingBarType.CorrectPredefinedType) {
				try {
					retVal = (PredefinedType != IfcReinforcingBarTypeEnum.USERDEFINED) || ((PredefinedType == IfcReinforcingBarTypeEnum.USERDEFINED) && EXISTS(this/* as IfcElementType*/.ElementType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcReinforcingBarType.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcReinforcingBarType.BendingShapeCodeProvided) {
				try {
					retVal = !EXISTS(BendingParameters) || EXISTS(BendingShapeCode);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcReinforcingBarType.BendingShapeCodeProvided' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcTypeProduct)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcReinforcingBarType.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcReinforcingBarType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcReinforcingBarType.BendingShapeCodeProvided))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcReinforcingBarType.BendingShapeCodeProvided", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcReinforcingBarType : IfcTypeProduct
	{
		public static readonly IfcReinforcingBarType CorrectPredefinedType = new IfcReinforcingBarType();
		public static readonly IfcReinforcingBarType BendingShapeCodeProvided = new IfcReinforcingBarType();
		protected IfcReinforcingBarType() {}
	}
}
