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
	public partial class IfcSweptDiskSolidPolygonal : IExpressValidatable
	{
		public enum IfcSweptDiskSolidPolygonalClause
		{
			CorrectRadii,
			DirectrixIsPolyline,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSweptDiskSolidPolygonalClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSweptDiskSolidPolygonalClause.CorrectRadii:
						retVal = !(Functions.EXISTS(FilletRadius)) || (FilletRadius >= this/* as IfcSweptDiskSolid*/.Radius);
						break;
					case IfcSweptDiskSolidPolygonalClause.DirectrixIsPolyline:
						retVal = Functions.TYPEOF(this/* as IfcSweptDiskSolid*/.Directrix).Contains("IFC4.IFCPOLYLINE");
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcSweptDiskSolidPolygonal>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSweptDiskSolidPolygonal.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcSweptDiskSolidPolygonalClause.CorrectRadii))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolidPolygonal.CorrectRadii", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcSweptDiskSolidPolygonalClause.DirectrixIsPolyline))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptDiskSolidPolygonal.DirectrixIsPolyline", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
