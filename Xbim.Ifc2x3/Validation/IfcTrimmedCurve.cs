using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class IfcTrimmedCurve : IExpressValidatable
	{
		public enum IfcTrimmedCurveClause
		{
			WR41,
			WR42,
			WR43,
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
					case IfcTrimmedCurveClause.WR41:
						retVal = (Functions.HIINDEX(Trim1) == 1) || (Functions.TYPEOF(Trim1.ItemAt(0)) != Functions.TYPEOF(Trim1.ItemAt(1)));
						break;
					case IfcTrimmedCurveClause.WR42:
						retVal = (Functions.HIINDEX(Trim2) == 1) || (Functions.TYPEOF(Trim2.ItemAt(0)) != Functions.TYPEOF(Trim2.ItemAt(1)));
						break;
					case IfcTrimmedCurveClause.WR43:
						retVal = !(Functions.TYPEOF(BasisCurve).Contains("IFC2X3.IFCBOUNDEDCURVE"));
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometryResource.IfcTrimmedCurve>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTrimmedCurve.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTrimmedCurveClause.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.WR41", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTrimmedCurveClause.WR42))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.WR42", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTrimmedCurveClause.WR43))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTrimmedCurve.WR43", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
