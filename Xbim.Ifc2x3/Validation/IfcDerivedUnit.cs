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

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDerivedUnit clause) {
			var retVal = false;
			if (clause == Where.IfcDerivedUnit.WR1) {
				try {
					retVal = (SIZEOF(Elements) > 1) || ((SIZEOF(Elements) == 1) && (Elements.ItemAt(0).Exponent != 1));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcDerivedUnit");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDerivedUnit.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcDerivedUnit.WR2) {
				try {
					retVal = (UnitType != IfcDerivedUnitEnum.USERDEFINED) || ((UnitType == IfcDerivedUnitEnum.USERDEFINED) && (EXISTS(this.UserDefinedType)));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.MeasureResource.IfcDerivedUnit");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDerivedUnit.WR2' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcDerivedUnit.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDerivedUnit.WR1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcDerivedUnit.WR2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDerivedUnit.WR2", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcDerivedUnit
	{
		public static readonly IfcDerivedUnit WR1 = new IfcDerivedUnit();
		public static readonly IfcDerivedUnit WR2 = new IfcDerivedUnit();
		protected IfcDerivedUnit() {}
	}
}
