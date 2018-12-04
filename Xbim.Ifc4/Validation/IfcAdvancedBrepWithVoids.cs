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
	public partial class IfcAdvancedBrepWithVoids : IExpressValidatable
	{
		public enum IfcAdvancedBrepWithVoidsClause
		{
			VoidsHaveAdvancedFaces,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAdvancedBrepWithVoidsClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAdvancedBrepWithVoidsClause.VoidsHaveAdvancedFaces:
						retVal = Functions.SIZEOF(Voids.Where(Vsh => Functions.SIZEOF(Vsh.CfsFaces.Where(Afs => (!(Functions.TYPEOF(Afs).Contains("IFC4.IFCADVANCEDFACE"))))) == 0)) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcAdvancedBrepWithVoids>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAdvancedBrepWithVoids.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcAdvancedBrepWithVoidsClause.VoidsHaveAdvancedFaces))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAdvancedBrepWithVoids.VoidsHaveAdvancedFaces", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
