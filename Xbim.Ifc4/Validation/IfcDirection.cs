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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcDirection : IExpressValidatable
	{
		public enum IfcDirectionClause
		{
			MagnitudeGreaterZero,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDirectionClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDirectionClause.MagnitudeGreaterZero:
						retVal = Functions.SIZEOF(DirectionRatios.Where(Tmp => Tmp != 0)) > 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcDirection>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcDirection.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDirectionClause.MagnitudeGreaterZero))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDirection.MagnitudeGreaterZero", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
