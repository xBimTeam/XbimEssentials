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
namespace Xbim.Ifc4.SharedBldgElements
{
	public partial class IfcSlabElementedCase : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSlabElementedCase clause) {
			var retVal = false;
			if (clause == Where.IfcSlabElementedCase.HasDecomposition) {
				try {
					retVal = HIINDEX(this/* as IfcObjectDefinition*/.IsDecomposedBy) > 0;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedBldgElements.IfcSlabElementedCase");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSlabElementedCase.HasDecomposition' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcSlab)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcSlabElementedCase.HasDecomposition))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSlabElementedCase.HasDecomposition", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSlabElementedCase : IfcSlab
	{
		public static readonly IfcSlabElementedCase HasDecomposition = new IfcSlabElementedCase();
		protected IfcSlabElementedCase() {}
	}
}
