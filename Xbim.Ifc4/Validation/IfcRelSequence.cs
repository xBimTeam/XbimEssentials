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
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelSequence clause) {
			var retVal = false;
			if (clause == Where.IfcRelSequence.AvoidInconsistentSequence) {
				try {
					retVal = !Object.ReferenceEquals(RelatingProcess, RelatedProcess);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelSequence.AvoidInconsistentSequence' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcRelSequence.CorrectSequenceType) {
				try {
					retVal = (SequenceType != IfcSequenceEnum.USERDEFINED) || ((SequenceType == IfcSequenceEnum.USERDEFINED) && EXISTS(UserDefinedSequenceType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelSequence.CorrectSequenceType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRelSequence.AvoidInconsistentSequence))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSequence.AvoidInconsistentSequence", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcRelSequence.CorrectSequenceType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSequence.CorrectSequenceType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRelSequence
	{
		public static readonly IfcRelSequence AvoidInconsistentSequence = new IfcRelSequence();
		public static readonly IfcRelSequence CorrectSequenceType = new IfcRelSequence();
		protected IfcRelSequence() {}
	}
}
