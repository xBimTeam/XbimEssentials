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
namespace Xbim.Ifc4.SharedFacilitiesElements
{
	public partial class IfcFurniture : IExpressValidatable
	{
		public enum IfcFurnitureClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcFurnitureClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcFurnitureClause.CorrectPredefinedType:
						retVal = !(Functions.EXISTS(PredefinedType)) || (PredefinedType != IfcFurnitureTypeEnum.USERDEFINED) || ((PredefinedType == IfcFurnitureTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcFurnitureClause.CorrectTypeAssigned:
						retVal = (Functions.SIZEOF(IsTypedBy) == 0) || (Functions.TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCFURNITURETYPE"));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.SharedFacilitiesElements.IfcFurniture>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcFurniture.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcFurnitureClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFurniture.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcFurnitureClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcFurniture.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
