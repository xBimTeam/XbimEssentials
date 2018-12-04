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
	public partial class IfcExtrudedAreaSolidTapered : IExpressValidatable
	{
		public enum IfcExtrudedAreaSolidTaperedClause
		{
			CorrectProfileAssignment,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcExtrudedAreaSolidTaperedClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcExtrudedAreaSolidTaperedClause.CorrectProfileAssignment:
						retVal = Functions.IfcTaperedSweptAreaProfiles(this/* as IfcSweptAreaSolid*/.SweptArea, this.EndSweptArea);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcExtrudedAreaSolidTapered>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcExtrudedAreaSolidTapered.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcExtrudedAreaSolidTaperedClause.CorrectProfileAssignment))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcExtrudedAreaSolidTapered.CorrectProfileAssignment", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
