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
	public partial class IfcTask : IExpressValidatable
	{
		public enum IfcTaskClause
		{
			HasName,
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTaskClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTaskClause.HasName:
						retVal = Functions.EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcTaskClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcTaskTypeEnum.USERDEFINED) || ((PredefinedType == IfcTaskTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProcessExtension.IfcTask>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTask.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcTaskClause.HasName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.HasName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTaskClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
