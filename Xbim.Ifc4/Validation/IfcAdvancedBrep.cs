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
	public partial class IfcAdvancedBrep : IExpressValidatable
	{
		public enum IfcAdvancedBrepClause
		{
			HasAdvancedFaces,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAdvancedBrepClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAdvancedBrepClause.HasAdvancedFaces:
						retVal = Functions.SIZEOF(this/* as IfcManifoldSolidBrep*/.Outer.CfsFaces.Where(Afs => (!(Functions.TYPEOF(Afs).Contains("IFC4.IFCADVANCEDFACE"))))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcAdvancedBrep>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAdvancedBrep.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcAdvancedBrepClause.HasAdvancedFaces))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAdvancedBrep.HasAdvancedFaces", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
