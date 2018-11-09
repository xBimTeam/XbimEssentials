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
namespace Xbim.Ifc2x3.HVACDomain
{
	public partial class IfcCoolingTowerType : IExpressValidatable
	{
		public enum IfcCoolingTowerTypeClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCoolingTowerTypeClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCoolingTowerTypeClause.WR1:
						retVal = (PredefinedType != IfcCoolingTowerTypeEnum.USERDEFINED) || ((PredefinedType == IfcCoolingTowerTypeEnum.USERDEFINED) && Functions.EXISTS(this/* as IfcElementType*/.ElementType));
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.HVACDomain.IfcCoolingTowerType>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCoolingTowerType.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcCoolingTowerTypeClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCoolingTowerType.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
