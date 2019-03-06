using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.IfcRail.SharedBldgElements
{
	public partial class IfcWindow : IExpressValidatable
	{
		public enum IfcWindowClause
		{
			CorrectStyleAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcWindowClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcWindowClause.CorrectStyleAssigned:
						retVal = (Functions.SIZEOF(IsTypedBy) == 0) || (Functions.TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFCWINDOWTYPE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.IfcRail.SharedBldgElements.IfcWindow>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcWindow.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcWindowClause.CorrectStyleAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindow.CorrectStyleAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
