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
namespace Xbim.Ifc4.MaterialResource
{
	public partial struct IfcCardinalPointReference : IExpressValidatable
	{
		public enum IfcCardinalPointReferenceClause
		{
			GreaterThanZero,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCardinalPointReferenceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCardinalPointReferenceClause.GreaterThanZero:
						retVal = this > 0;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.MaterialResource.IfcCardinalPointReference");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcCardinalPointReference.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCardinalPointReferenceClause.GreaterThanZero))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCardinalPointReference.GreaterThanZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
