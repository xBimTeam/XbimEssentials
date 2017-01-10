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
	public partial class IfcUnitaryControlElementType : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcUnitaryControlElementType clause) {
			var retVal = false;
			if (clause == Where.IfcUnitaryControlElementType.CorrectPredefinedType) {
				try {
					retVal = (PredefinedType != IfcUnitaryControlElementTypeEnum.USERDEFINED) || ((PredefinedType == IfcUnitaryControlElementTypeEnum.USERDEFINED) && EXISTS(this/* as IfcElementType*/.ElementType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.BuildingControlsDomain.IfcUnitaryControlElementType");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcUnitaryControlElementType.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
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
			if (!ValidateClause(Where.IfcUnitaryControlElementType.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcUnitaryControlElementType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcUnitaryControlElementType : IfcTypeProduct
	{
		public static readonly IfcUnitaryControlElementType CorrectPredefinedType = new IfcUnitaryControlElementType();
		protected IfcUnitaryControlElementType() {}
	}
}
