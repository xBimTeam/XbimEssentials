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
namespace Xbim.Ifc4.BuildingControlsDomain
{
	public partial class IfcAlarmType : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAlarmType clause) {
			var retVal = false;
			if (clause == Where.IfcAlarmType.CorrectPredefinedType) {
				try {
					retVal = (PredefinedType != IfcAlarmTypeEnum.USERDEFINED) || ((PredefinedType == IfcAlarmTypeEnum.USERDEFINED) && EXISTS(this/* as IfcElementType*/.ElementType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.BuildingControlsDomain.IfcAlarmType");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAlarmType.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcAlarmType.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAlarmType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAlarmType : IfcTypeProduct
	{
		public static readonly IfcAlarmType CorrectPredefinedType = new IfcAlarmType();
		protected IfcAlarmType() {}
	}
}
