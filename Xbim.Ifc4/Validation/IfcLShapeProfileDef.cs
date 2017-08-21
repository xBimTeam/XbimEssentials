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
namespace Xbim.Ifc4.ProfileResource
{
	public partial class IfcLShapeProfileDef : IExpressValidatable
	{
		public enum IfcLShapeProfileDefClause
		{
			ValidThickness,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcLShapeProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcLShapeProfileDefClause.ValidThickness:
						retVal = (Thickness < Depth) && (!(Functions.EXISTS(Width)) || (Thickness < Width));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProfileResource.IfcLShapeProfileDef>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcLShapeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcLShapeProfileDefClause.ValidThickness))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcLShapeProfileDef.ValidThickness", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
