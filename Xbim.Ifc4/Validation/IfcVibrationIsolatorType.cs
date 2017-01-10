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
	public partial class IfcVibrationIsolatorType : IExpressValidatable
	{
		public enum IfcVibrationIsolatorTypeClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcVibrationIsolatorTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcVibrationIsolatorTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcVibrationIsolatorTypeEnum.USERDEFINED) || ((PredefinedType == IfcVibrationIsolatorTypeEnum.USERDEFINED) && EXISTS(this/* as IfcElementType*/.ElementType));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcVibrationIsolatorType");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcVibrationIsolatorType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcVibrationIsolatorTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcVibrationIsolatorType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
