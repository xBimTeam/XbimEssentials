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
namespace Xbim.Ifc4.SharedComponentElements
{
	public partial class IfcDiscreteAccessoryType : IExpressValidatable
	{
		public enum IfcDiscreteAccessoryTypeClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDiscreteAccessoryTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDiscreteAccessoryTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcDiscreteAccessoryTypeEnum.USERDEFINED) || ((PredefinedType == IfcDiscreteAccessoryTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcElementType*/.ElementType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.SharedComponentElements.IfcDiscreteAccessoryType>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcDiscreteAccessoryType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcDiscreteAccessoryTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDiscreteAccessoryType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
