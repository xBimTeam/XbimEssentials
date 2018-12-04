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
	public partial class IfcSpatialZoneType : IExpressValidatable
	{
		public enum IfcSpatialZoneTypeClause
		{
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcSpatialZoneTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcSpatialZoneTypeClause.CorrectPredefinedType:
						retVal = (PredefinedType != IfcSpatialZoneTypeEnum.USERDEFINED) || ((PredefinedType == IfcSpatialZoneTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcSpatialElementType*/.ElementType));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ProductExtension.IfcSpatialZoneType>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcSpatialZoneType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcSpatialZoneTypeClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSpatialZoneType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
