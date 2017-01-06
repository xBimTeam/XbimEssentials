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
namespace Xbim.Ifc4.HvacDomain
{
	public partial class IfcAirTerminalBox : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcAirTerminalBox");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAirTerminalBox clause) {
			var retVal = false;
			if (clause == Where.IfcAirTerminalBox.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcAirTerminalBoxTypeEnum.USERDEFINED) || ((PredefinedType == IfcAirTerminalBoxTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAirTerminalBox.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAirTerminalBox.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCAIRTERMINALBOXTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAirTerminalBox.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcAirTerminalBox.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAirTerminalBox.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAirTerminalBox.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAirTerminalBox.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAirTerminalBox : IfcProduct
	{
		public static readonly IfcAirTerminalBox CorrectPredefinedType = new IfcAirTerminalBox();
		public static readonly IfcAirTerminalBox CorrectTypeAssigned = new IfcAirTerminalBox();
		protected IfcAirTerminalBox() {}
	}
}
