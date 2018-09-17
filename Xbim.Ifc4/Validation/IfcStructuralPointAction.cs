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
namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	public partial class IfcStructuralPointAction : IExpressValidatable
	{
		public enum IfcStructuralPointActionClause
		{
			SuitableLoadType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStructuralPointActionClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStructuralPointActionClause.SuitableLoadType:
						retVal = Functions.SIZEOF(Functions.NewTypesArray("IFC4.IFCSTRUCTURALLOADSINGLEFORCE", "IFC4.IFCSTRUCTURALLOADSINGLEDISPLACEMENT") * Functions.TYPEOF(this/* as IfcStructuralActivity*/.AppliedLoad)) == 1;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralPointAction");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcStructuralPointAction.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcStructuralPointActionClause.SuitableLoadType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStructuralPointAction.SuitableLoadType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
