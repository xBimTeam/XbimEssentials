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
namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	public partial class IfcStructuralLoadCase : IExpressValidatable
	{
		public enum IfcStructuralLoadCaseClause
		{
			IsLoadCasePredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralLoadCaseClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralLoadCaseClause.IsLoadCasePredefinedType:
						retVal = this/* as IfcStructuralLoadGroup*/.PredefinedType == IfcLoadGroupTypeEnum.LOAD_CASE;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralLoadCase");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralLoadCase.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralLoadCaseClause.IsLoadCasePredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralLoadCase.IsLoadCasePredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
