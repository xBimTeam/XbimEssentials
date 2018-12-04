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
namespace Xbim.Ifc4.TopologyResource
{
	public partial class IfcFace : IExpressValidatable
	{
		public enum IfcFaceClause
		{
			HasOuterBound,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFaceClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFaceClause.HasOuterBound:
						retVal = Functions.SIZEOF(Bounds.Where(temp => Functions.TYPEOF(temp).Contains("IFC4.IFCFACEOUTERBOUND"))) <= 1;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.TopologyResource.IfcFace>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcFace.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcFaceClause.HasOuterBound))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFace.HasOuterBound", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
