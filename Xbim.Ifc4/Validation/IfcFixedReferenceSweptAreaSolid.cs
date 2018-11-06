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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcFixedReferenceSweptAreaSolid : IExpressValidatable
	{
		public enum IfcFixedReferenceSweptAreaSolidClause
		{
			DirectrixBounded,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFixedReferenceSweptAreaSolidClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFixedReferenceSweptAreaSolidClause.DirectrixBounded:
						retVal = (Functions.EXISTS(StartParam) && Functions.EXISTS(EndParam)) || (Functions.SIZEOF(Functions.NewTypesArray("IFC4.IFCCONIC", "IFC4.IFCBOUNDEDCURVE") * Functions.TYPEOF(Directrix)) == 1);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcFixedReferenceSweptAreaSolid>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcFixedReferenceSweptAreaSolid.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcFixedReferenceSweptAreaSolidClause.DirectrixBounded))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFixedReferenceSweptAreaSolid.DirectrixBounded", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
