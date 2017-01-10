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
	public partial class IfcFlowMeterType : IExpressValidatable
	{
		public enum IfcFlowMeterTypeClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFlowMeterTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFlowMeterTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcFlowMeterTypeEnum.USERDEFINED) || ((PredefinedType == IfcFlowMeterTypeEnum.USERDEFINED) && EXISTS(this/* as IfcElementType*/.ElementType));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcFlowMeterType");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcFlowMeterType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcFlowMeterTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFlowMeterType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
