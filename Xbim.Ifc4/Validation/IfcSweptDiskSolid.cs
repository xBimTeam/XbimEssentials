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
namespace Xbim.Ifc4.GeometricModelResource
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
						retVal = (!EXISTS(InnerRadius)) || (Radius > InnerRadius);
						break;
					case IfcSweptDiskSolidClause.DirectrixBounded:
						retVal = (EXISTS(StartParam) && EXISTS(EndParam)) || (SIZEOF(NewArray("IFC4.IFCCONIC", "IFC4.IFCBOUNDEDCURVE") * TYPEOF(Directrix)) == 1);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcSweptDiskSolid");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcSweptDiskSolid.{0}' for #{1}.", clause,EntityLabel), ex);
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
