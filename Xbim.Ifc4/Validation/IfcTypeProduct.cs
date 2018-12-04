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
namespace Xbim.Ifc4.Kernel
{
	public partial class IfcTypeProduct : IExpressValidatable
	{
		public enum IfcTypeProductClause
		{
			ApplicableOccurrence,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTypeProductClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTypeProductClause.ApplicableOccurrence:
						retVal = !(Functions.EXISTS(this/* as IfcTypeObject*/.Types.ItemAt(0))) || (Functions.SIZEOF(this/* as IfcTypeObject*/.Types.ItemAt(0).RelatedObjects.Where(temp => !(Functions.TYPEOF(temp).Contains("IFC4.IFCPRODUCT")))) == 0);
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.Kernel.IfcTypeProduct>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTypeProduct.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcTypeProductClause.ApplicableOccurrence))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTypeProduct.ApplicableOccurrence", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
