using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.RepresentationResource
{
	public partial class IfcProductDefinitionShape : IExpressValidatable
	{
		public enum IfcProductDefinitionShapeClause
		{
			WR11,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcProductDefinitionShapeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcProductDefinitionShapeClause.WR11:
						retVal = Functions.SIZEOF(Representations.Where(temp => (!(Functions.TYPEOF(temp).Contains("IFC2X3.IFCSHAPEMODEL"))))) == 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.RepresentationResource.IfcProductDefinitionShape>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcProductDefinitionShape.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcProductDefinitionShapeClause.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProductDefinitionShape.WR11", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
