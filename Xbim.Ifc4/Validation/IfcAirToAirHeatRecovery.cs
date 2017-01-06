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
	public partial class IfcAirToAirHeatRecovery : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcAirToAirHeatRecovery");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAirToAirHeatRecovery clause) {
			var retVal = false;
			if (clause == Where.IfcAirToAirHeatRecovery.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcAirToAirHeatRecoveryTypeEnum.USERDEFINED) || ((PredefinedType == IfcAirToAirHeatRecoveryTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAirToAirHeatRecovery.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcAirToAirHeatRecovery.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ToArray()[0].RelatingType).Contains("IFC4.IFCAIRTOAIRHEATRECOVERYTYPE"));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAirToAirHeatRecovery.CorrectTypeAssigned' for #{EntityLabel}.", ex);
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
			if (!ValidateClause(Where.IfcAirToAirHeatRecovery.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAirToAirHeatRecovery.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcAirToAirHeatRecovery.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAirToAirHeatRecovery.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAirToAirHeatRecovery : IfcProduct
	{
		public static readonly IfcAirToAirHeatRecovery CorrectPredefinedType = new IfcAirToAirHeatRecovery();
		public static readonly IfcAirToAirHeatRecovery CorrectTypeAssigned = new IfcAirToAirHeatRecovery();
		protected IfcAirToAirHeatRecovery() {}
	}
}
