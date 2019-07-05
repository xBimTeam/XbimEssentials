using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ElectricalDomain
{
	public partial class IfcCableCarrierFittingType : IExpressValidatable
	{
		public enum IfcCableCarrierFittingTypeClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCableCarrierFittingTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCableCarrierFittingTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcCableCarrierFittingTypeEnum.USERDEFINED) || ((PredefinedType == IfcCableCarrierFittingTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcElementType*/.ElementType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ElectricalDomain.IfcCableCarrierFittingType>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCableCarrierFittingType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcCableCarrierFittingTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCableCarrierFittingType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
