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
	public partial class IfcStructuralPointReaction : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralPointReaction");

		/// <summary>
		/// Tests the express where clause SuitableLoadType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SuitableLoadType() {
			var retVal = false;
			try {
				retVal = SIZEOF(NewArray("IFC4.IFCSTRUCTURALLOADSINGLEFORCE", "IFC4.IFCSTRUCTURALLOADSINGLEDISPLACEMENT") * TYPEOF(this/* as IfcStructuralActivity*/.AppliedLoad)) == 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SuitableLoadType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!SuitableLoadType())
				yield return new ValidationResult() { Item = this, IssueSource = "SuitableLoadType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
