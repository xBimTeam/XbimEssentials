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
	public partial class IfcRelSequence : IExpressValidatable
	{
		public enum IfcRelSequenceClause
		{
			AvoidInconsistentSequence,
			CorrectSequenceType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelSequenceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelSequenceClause.AvoidInconsistentSequence:
						retVal = !Object.ReferenceEquals(RelatingProcess, RelatedProcess);
						break;
					case IfcRelSequenceClause.CorrectSequenceType:
						retVal = (SequenceType != IfcSequenceEnum.USERDEFINED) || ((SequenceType == IfcSequenceEnum.USERDEFINED) && Functions.EXISTS(UserDefinedSequenceType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProcessExtension.IfcRelSequence>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelSequence.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRelSequenceClause.AvoidInconsistentSequence))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSequence.AvoidInconsistentSequence", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRelSequenceClause.CorrectSequenceType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSequence.CorrectSequenceType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
