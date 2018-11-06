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
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcRelAssociatesMaterial : IExpressValidatable
	{
		public enum IfcRelAssociatesMaterialClause
		{
			NoVoidElement,
			AllowedElements,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRelAssociatesMaterialClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRelAssociatesMaterialClause.NoVoidElement:
						retVal = Functions.SIZEOF(this/* as IfcRelAssociates*/.RelatedObjects.Where(temp => (Functions.TYPEOF(temp).Contains("IFC4.IFCFEATUREELEMENTSUBTRACTION")) || (Functions.TYPEOF(temp).Contains("IFC4.IFCVIRTUALELEMENT")))) == 0;
						break;
					case IfcRelAssociatesMaterialClause.AllowedElements:
						retVal = Functions.SIZEOF(this/* as IfcRelAssociates*/.RelatedObjects.Where(temp => (Functions.SIZEOF(Functions.TYPEOF(temp) * Functions.NewTypesArray("IFC4.IFCELEMENT", "IFC4.IFCELEMENTTYPE", "IFC4.IFCWINDOWSTYLE", "IFC4.IFCDOORSTYLE", "IFC4.IFCSTRUCTURALMEMBER", "IFC4.IFCPORT")) == 0))) == 0;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProductExtension.IfcRelAssociatesMaterial>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcRelAssociatesMaterial.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRelAssociatesMaterialClause.NoVoidElement))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssociatesMaterial.NoVoidElement", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcRelAssociatesMaterialClause.AllowedElements))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelAssociatesMaterial.AllowedElements", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
