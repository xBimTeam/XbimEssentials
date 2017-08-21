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
	public partial class IfcStyledRepresentation : IExpressValidatable
	{
		public enum IfcStyledRepresentationClause
		{
			OnlyStyledItems,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcStyledRepresentationClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcStyledRepresentationClause.OnlyStyledItems:
						retVal = Functions.SIZEOF(this/* as IfcRepresentation*/.Items.Where(temp => (!(Functions.TYPEOF(temp).Contains("IFC4.IFCSTYLEDITEM"))))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.RepresentationResource.IfcStyledRepresentation>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcStyledRepresentation.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcStyledRepresentationClause.OnlyStyledItems))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStyledRepresentation.OnlyStyledItems", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
