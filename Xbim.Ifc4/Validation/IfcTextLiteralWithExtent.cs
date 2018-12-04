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
namespace Xbim.Ifc4.PresentationDefinitionResource
{
	public partial class IfcTextLiteralWithExtent : IExpressValidatable
	{
		public enum IfcTextLiteralWithExtentClause
		{
			WR31,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTextLiteralWithExtentClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTextLiteralWithExtentClause.WR31:
						retVal = !(Functions.TYPEOF(Extent).Contains("IFC4.IFCPLANARBOX"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.PresentationDefinitionResource.IfcTextLiteralWithExtent>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTextLiteralWithExtent.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTextLiteralWithExtentClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextLiteralWithExtent.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
