using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Kernel
{
	public partial class IfcRelSequence : IExpressValidatable
	{
		public enum IfcRelSequenceClause
		{
			WR1,
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
					case IfcRelSequenceClause.WR1:
						retVal = !Object.ReferenceEquals(RelatingProcess, RelatedProcess);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.Kernel.IfcRelSequence>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelSequence.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRelSequenceClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelSequence.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
