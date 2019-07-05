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
	public partial class IfcTaskType : IExpressValidatable
	{
		public enum IfcTaskTypeClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTaskTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTaskTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcTaskTypeEnum.USERDEFINED) || ((PredefinedType == IfcTaskTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcTypeProcess*/.ProcessType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProcessExtension.IfcTaskType>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTaskType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcTaskTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTaskType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
