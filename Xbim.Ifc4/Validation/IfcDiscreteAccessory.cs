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
	public partial class IfcDiscreteAccessory : IExpressValidatable
	{
		public enum IfcDiscreteAccessoryClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDiscreteAccessoryClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDiscreteAccessoryClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcDiscreteAccessoryTypeEnum.USERDEFINED) || ((PredefinedType == IfcDiscreteAccessoryTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcDiscreteAccessoryClause.CorrectTypeAssigned:
						retVal = (Functions.SIZEOF(IsTypedBy) == 0) || (Functions.TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCDISCRETEACCESSORYTYPE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.SharedComponentElements.IfcDiscreteAccessory>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcDiscreteAccessory.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcDiscreteAccessoryClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDiscreteAccessory.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDiscreteAccessoryClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDiscreteAccessory.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
