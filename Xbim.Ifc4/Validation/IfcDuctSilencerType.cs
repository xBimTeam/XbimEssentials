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
	public partial class IfcDuctSilencerType : IExpressValidatable
	{
		public enum IfcDuctSilencerTypeClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDuctSilencerTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDuctSilencerTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcDuctSilencerTypeEnum.USERDEFINED) || ((PredefinedType == IfcDuctSilencerTypeEnum.USERDEFINED) && EXISTS(this/* as IfcElementType*/.ElementType));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcDuctSilencerType");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDuctSilencerType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcDuctSilencerTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDuctSilencerType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
