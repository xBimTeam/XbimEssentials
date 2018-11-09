using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcBoundaryCurve : IExpressValidatable
	{
		public enum IfcBoundaryCurveClause
		{
			IsClosed,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcBoundaryCurveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcBoundaryCurveClause.IsClosed:
						retVal = this/* as IfcCompositeCurve*/.ClosedCurve.AsBool();
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcBoundaryCurve>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcBoundaryCurve.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcBoundaryCurveClause.IsClosed))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBoundaryCurve.IsClosed", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
