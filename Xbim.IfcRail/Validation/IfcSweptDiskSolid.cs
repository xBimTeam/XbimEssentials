using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.IfcRail.GeometricModelResource
{
	public partial class IfcSweptDiskSolid : IExpressValidatable
	{
		public enum IfcSweptDiskSolidClause
		{
			DirectrixDim,
			InnerRadiusSize,
			DirectrixBounded,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSweptDiskSolidClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSweptDiskSolidClause.DirectrixDim:
						retVal = Directrix.Dim == 3;
						break;
					case IfcSweptDiskSolidClause.InnerRadiusSize:
						retVal = (!Functions.EXISTS(InnerRadius)) || (Radius > InnerRadius);
						break;
					case IfcSweptDiskSolidClause.DirectrixBounded:
						retVal = (Functions.EXISTS(StartParam) && Functions.EXISTS(EndParam)) || (Functions.SIZEOF(Functions.NewTypesArray("IFCCONIC", "IFCBOUNDEDCURVE") * Functions.TYPEOF(Directrix)) == 1);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.IfcRail.GeometricModelResource.IfcSweptDiskSolid>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSweptDiskSolid.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcSweptDiskSolidClause.DirectrixDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolid.DirectrixDim", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSweptDiskSolidClause.InnerRadiusSize))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolid.InnerRadiusSize", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSweptDiskSolidClause.DirectrixBounded))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolid.DirectrixBounded", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
