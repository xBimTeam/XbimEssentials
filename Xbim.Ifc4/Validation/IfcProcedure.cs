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
namespace Xbim.Ifc4.ProcessExtension
{
	public partial class IfcProcedure : IExpressValidatable
	{
		public enum IfcProcedureClause
		{
			HasName,
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcProcedureClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcProcedureClause.HasName:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcProcedureClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcProcedureTypeEnum.USERDEFINED) || ((PredefinedType == IfcProcedureTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProcessExtension.IfcProcedure>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcProcedure.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcProcedureClause.HasName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.HasName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProcedureClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
