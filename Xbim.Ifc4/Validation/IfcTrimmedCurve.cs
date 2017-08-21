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
	public partial class IfcTrimmedCurve : IExpressValidatable
	{
		public enum IfcTrimmedCurveClause
		{
			Trim1ValuesConsistent,
			Trim2ValuesConsistent,
			NoTrimOfBoundedCurves,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTrimmedCurveClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTrimmedCurveClause.Trim1ValuesConsistent:
						retVal = (Functions.HIINDEX(Trim1) == 1) || (Functions.TYPEOF(Trim1.ItemAt(0)) != Functions.TYPEOF(Trim1.ItemAt(1)));
						break;
					case IfcTrimmedCurveClause.Trim2ValuesConsistent:
						retVal = (Functions.HIINDEX(Trim2) == 1) || (Functions.TYPEOF(Trim2.ItemAt(0)) != Functions.TYPEOF(Trim2.ItemAt(1)));
						break;
					case IfcTrimmedCurveClause.NoTrimOfBoundedCurves:
						retVal = !(Functions.TYPEOF(BasisCurve).Contains("IFC4.IFCBOUNDEDCURVE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcTrimmedCurve>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTrimmedCurve.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTrimmedCurveClause.Trim1ValuesConsistent))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.Trim1ValuesConsistent", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTrimmedCurveClause.Trim2ValuesConsistent))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.Trim2ValuesConsistent", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTrimmedCurveClause.NoTrimOfBoundedCurves))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.NoTrimOfBoundedCurves", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
