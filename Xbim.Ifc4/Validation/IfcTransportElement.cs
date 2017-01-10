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
namespace Xbim.Ifc4.ProductExtension
{
	public partial class IfcTransportElement : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTransportElement clause) {
			var retVal = false;
			if (clause == Where.IfcTransportElement.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcTransportElementTypeEnum.USERDEFINED) || ((PredefinedType == IfcTransportElementTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcTransportElement");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTransportElement.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTransportElement.CorrectTypeAssigned) {
				try {
					retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCTRANSPORTELEMENTTYPE"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcTransportElement");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTransportElement.CorrectTypeAssigned' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcTransportElement.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTransportElement.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTransportElement.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTransportElement.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcTransportElement : IfcProduct
	{
		public static readonly IfcTransportElement CorrectPredefinedType = new IfcTransportElement();
		public static readonly IfcTransportElement CorrectTypeAssigned = new IfcTransportElement();
		protected IfcTransportElement() {}
	}
}
