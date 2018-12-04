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
	public partial class IfcGeometricSet : IExpressValidatable
	{
		public enum IfcGeometricSetClause
		{
			ConsistentDim,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcGeometricSetClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcGeometricSetClause.ConsistentDim:
						retVal = Functions.SIZEOF(Elements.Where(Temp => Temp.Dim != Elements.ItemAt(0).Dim)) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometricModelResource.IfcGeometricSet>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcGeometricSet.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcGeometricSetClause.ConsistentDim))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcGeometricSet.ConsistentDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
