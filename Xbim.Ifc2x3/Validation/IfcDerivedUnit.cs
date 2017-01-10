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
namespace Xbim.Ifc2x3.MeasureResource
{
	public partial class IfcDerivedUnit : IExpressValidatable
	{
		public enum IfcDerivedUnitClause
		{
			WR1,
			WR2,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDerivedUnitClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDerivedUnitClause.WR1:
						retVal = (SIZEOF(Elements) > 1) || ((SIZEOF(Elements) == 1) && (Elements.ItemAt(0).Exponent != 1));
						break;
					case IfcDerivedUnitClause.WR2:
						retVal = (UnitType != IfcDerivedUnitEnum.USERDEFINED) || ((UnitType == IfcDerivedUnitEnum.USERDEFINED) && (EXISTS(this.UserDefinedType)));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcDerivedUnit");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDerivedUnit.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDerivedUnitClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDerivedUnit.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDerivedUnitClause.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDerivedUnit.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
