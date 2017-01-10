using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
using static Xbim.Ifc4.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ArchitectureDomain
{
	public partial class IfcDoorLiningProperties : IExpressValidatable
	{
		public enum IfcDoorLiningPropertiesClause
		{
			WR31,
			WR32,
			WR33,
			WR34,
			WR35,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDoorLiningPropertiesClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDoorLiningPropertiesClause.WR31:
						retVal = !(EXISTS(LiningDepth) && !(EXISTS(LiningThickness)));
						break;
					case IfcDoorLiningPropertiesClause.WR32:
						retVal = !(EXISTS(ThresholdDepth) && !(EXISTS(ThresholdThickness)));
						break;
					case IfcDoorLiningPropertiesClause.WR33:
						retVal = (EXISTS(TransomOffset) && EXISTS(TransomThickness)) ^ (!(EXISTS(TransomOffset)) && !(EXISTS(TransomThickness)));
						break;
					case IfcDoorLiningPropertiesClause.WR34:
						retVal = (EXISTS(CasingDepth) && EXISTS(CasingThickness)) ^ (!(EXISTS(CasingDepth)) && !(EXISTS(CasingThickness)));
						break;
					case IfcDoorLiningPropertiesClause.WR35:
						retVal = (EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0))) && ((TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC4.IFCDOORTYPE")) || (TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC4.IFCDOORSTYLE")));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ArchitectureDomain.IfcDoorLiningProperties");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDoorLiningProperties.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDoorLiningPropertiesClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDoorLiningProperties.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDoorLiningPropertiesClause.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDoorLiningProperties.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDoorLiningPropertiesClause.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDoorLiningProperties.WR33", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDoorLiningPropertiesClause.WR34))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDoorLiningProperties.WR34", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDoorLiningPropertiesClause.WR35))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDoorLiningProperties.WR35", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
