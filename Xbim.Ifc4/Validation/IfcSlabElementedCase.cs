using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcSlabElementedCase : IExpressValidatable
	{
		public enum IfcSlabElementedCaseClause
		{
			HasDecomposition,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSlabElementedCaseClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSlabElementedCaseClause.HasDecomposition:
						retVal = Functions.HIINDEX(this/* as IfcObjectDefinition*/.IsDecomposedBy) > 0;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcSlabElementedCase");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSlabElementedCase.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcSlabElementedCaseClause.HasDecomposition))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSlabElementedCase.HasDecomposition", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
