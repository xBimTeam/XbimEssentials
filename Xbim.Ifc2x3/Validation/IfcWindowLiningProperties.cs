using System;
using log4net;
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
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.SharedBldgElements
{
	public partial class IfcWindowLiningProperties : IExpressValidatable
	{
		public enum IfcWindowLiningPropertiesClause
		{
			WR31,
			WR32,
			WR33,
			WR34,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcWindowLiningPropertiesClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcWindowLiningPropertiesClause.WR31:
						retVal = !(!(EXISTS(LiningDepth)) && EXISTS(LiningThickness));
						break;
					case IfcWindowLiningPropertiesClause.WR32:
						retVal = !(!(EXISTS(FirstTransomOffset)) && EXISTS(SecondTransomOffset));
						break;
					case IfcWindowLiningPropertiesClause.WR33:
						retVal = !(!(EXISTS(FirstMullionOffset)) && EXISTS(SecondMullionOffset));
						break;
					case IfcWindowLiningPropertiesClause.WR34:
						retVal = EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)) && (TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC2X3.IFCWINDOWSTYLE"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.SharedBldgElements.IfcWindowLiningProperties");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcWindowLiningProperties.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcWindowLiningPropertiesClause.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowLiningProperties.WR31", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcWindowLiningPropertiesClause.WR32))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowLiningProperties.WR32", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcWindowLiningPropertiesClause.WR33))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowLiningProperties.WR33", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcWindowLiningPropertiesClause.WR34))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcWindowLiningProperties.WR34", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
