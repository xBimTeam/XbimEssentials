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
namespace Xbim.Ifc4.HvacDomain
{
	public partial class IfcPump : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPump clause) {
			var retVal = false;
			if (clause == Where.IfcPump.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcPumpTypeEnum.USERDEFINED) || ((PredefinedType == IfcPumpTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcPump");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPump.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcPump.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCPUMPTYPE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcPump");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcPump.CorrectTypeAssigned' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcProduct)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcPump.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPump.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcPump.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPump.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPump : IfcProduct
	{
		public static readonly IfcPump CorrectPredefinedType = new IfcPump();
		public static readonly IfcPump CorrectTypeAssigned = new IfcPump();
		protected IfcPump() {}
	}
}
