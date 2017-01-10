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
namespace Xbim.Ifc2x3.HVACDomain
{
	public partial class IfcVibrationIsolatorType : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcVibrationIsolatorType clause) {
			var retVal = false;
			if (clause == Where.IfcVibrationIsolatorType.WR1) {
				try {
					retVal = (PredefinedType != IfcVibrationIsolatorTypeEnum.USERDEFINED) || ((PredefinedType == IfcVibrationIsolatorTypeEnum.USERDEFINED) && EXISTS(this/* as IfcElementType*/.ElementType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.HVACDomain.IfcVibrationIsolatorType");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcVibrationIsolatorType.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcTypeProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcVibrationIsolatorType.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcVibrationIsolatorType.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcVibrationIsolatorType : IfcTypeProduct
	{
		public new static readonly IfcVibrationIsolatorType WR1 = new IfcVibrationIsolatorType();
		protected IfcVibrationIsolatorType() {}
	}
}
