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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcGeometricRepresentationContext : IExpressValidatable
	{
		public enum IfcGeometricRepresentationContextClause
		{
			North2D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcGeometricRepresentationContextClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcGeometricRepresentationContextClause.North2D:
						retVal = !(EXISTS(TrueNorth)) || (HIINDEX(TrueNorth.DirectionRatios) == 2);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcGeometricRepresentationContext");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcGeometricRepresentationContext.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcGeometricRepresentationContextClause.North2D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricRepresentationContext.North2D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
