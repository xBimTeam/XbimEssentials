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
	public partial class IfcGeometricCurveSet : IExpressValidatable
	{
		public enum IfcGeometricCurveSetClause
		{
			NoSurfaces,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcGeometricCurveSetClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcGeometricCurveSetClause.NoSurfaces:
						retVal = Functions.SIZEOF(this/* as IfcGeometricSet*/.Elements.Where(Temp => Functions.TYPEOF(Temp).Contains("IFC4.IFCSURFACE"))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcGeometricCurveSet>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcGeometricCurveSet.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcGeometricCurveSetClause.NoSurfaces))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricCurveSet.NoSurfaces", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
