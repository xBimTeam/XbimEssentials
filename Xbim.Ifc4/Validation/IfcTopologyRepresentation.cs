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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcTopologyRepresentation : IExpressValidatable
	{
		public enum IfcTopologyRepresentationClause
		{
			WR21,
			WR22,
			WR23,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTopologyRepresentationClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTopologyRepresentationClause.WR21:
						retVal = Functions.SIZEOF(this/* as IfcRepresentation*/.Items.Where(temp => !(Functions.TYPEOF(temp).Contains("IFC4.IFCTOPOLOGICALREPRESENTATIONITEM")))) == 0;
						break;
					case IfcTopologyRepresentationClause.WR22:
						retVal = Functions.EXISTS(this/* as IfcRepresentation*/.RepresentationType);
						break;
					case IfcTopologyRepresentationClause.WR23:
						retVal = Functions.IfcTopologyRepresentationTypes(this/* as IfcRepresentation*/.RepresentationType, this/* as IfcRepresentation*/.Items);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.RepresentationResource.IfcTopologyRepresentation>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTopologyRepresentation.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcTopologyRepresentationClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTopologyRepresentation.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTopologyRepresentationClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTopologyRepresentation.WR22", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcTopologyRepresentationClause.WR23))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTopologyRepresentation.WR23", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
