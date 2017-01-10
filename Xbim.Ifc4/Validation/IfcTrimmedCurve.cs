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
						retVal = (HIINDEX(Trim1) == 1) || (TYPEOF(Trim1.ItemAt(0)) != TYPEOF(Trim1.ItemAt(1)));
						break;
					case IfcTrimmedCurveClause.Trim2ValuesConsistent:
						retVal = (HIINDEX(Trim2) == 1) || (TYPEOF(Trim2.ItemAt(0)) != TYPEOF(Trim2.ItemAt(1)));
						break;
					case IfcTrimmedCurveClause.NoTrimOfBoundedCurves:
						retVal = !(TYPEOF(BasisCurve).Contains("IFC4.IFCBOUNDEDCURVE"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcTrimmedCurve");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTrimmedCurve.{0}' for #{1}.", clause,EntityLabel), ex);
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
