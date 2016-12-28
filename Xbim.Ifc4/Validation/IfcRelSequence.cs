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
namespace Xbim.Ifc4.ProcessExtension
{
	public partial class IfcRelSequence : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProcessExtension.IfcRelSequence");

		/// <summary>
		/// Tests the express where clause AvoidInconsistentSequence
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool AvoidInconsistentSequence() {
			var retVal = false;
			try {
				retVal = !Object.ReferenceEquals(RelatingProcess, RelatedProcess);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'AvoidInconsistentSequence' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause CorrectSequenceType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectSequenceType() {
			var retVal = false;
			try {
				retVal = (SequenceType != IfcSequenceEnum.USERDEFINED) || ((SequenceType == IfcSequenceEnum.USERDEFINED) && EXISTS(UserDefinedSequenceType));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectSequenceType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!AvoidInconsistentSequence())
				yield return new ValidationResult() { Item = this, IssueSource = "AvoidInconsistentSequence", IssueType = ValidationFlags.EntityWhereClauses };
			if (!CorrectSequenceType())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectSequenceType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
