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
namespace Xbim.Ifc4.ElectricalDomain
{
	public partial class IfcJunctionBox : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcJunctionBox clause) {
			var retVal = false;
			if (clause == Where.IfcJunctionBox.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcJunctionBoxTypeEnum.USERDEFINED) || ((PredefinedType == IfcJunctionBoxTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcJunctionBox");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcJunctionBox.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcJunctionBox.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCJUNCTIONBOXTYPE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcJunctionBox");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcJunctionBox.CorrectTypeAssigned' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcJunctionBox.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcJunctionBox.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcJunctionBox.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcJunctionBox.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcJunctionBox : IfcProduct
	{
		public static readonly IfcJunctionBox CorrectPredefinedType = new IfcJunctionBox();
		public static readonly IfcJunctionBox CorrectTypeAssigned = new IfcJunctionBox();
		protected IfcJunctionBox() {}
	}
}
