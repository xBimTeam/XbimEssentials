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
	public partial class IfcSpaceHeaterType : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.HvacDomain.IfcSpaceHeaterType");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSpaceHeaterType clause) {
			var retVal = false;
			if (clause == Where.IfcSpaceHeaterType.CorrectPredefinedType) {
				try {
					retVal = (PredefinedType != IfcSpaceHeaterTypeEnum.USERDEFINED) || ((PredefinedType == IfcSpaceHeaterTypeEnum.USERDEFINED) && EXISTS(this/* as IfcElementType*/.ElementType));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSpaceHeaterType.CorrectPredefinedType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcTypeProduct)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcSpaceHeaterType.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSpaceHeaterType.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSpaceHeaterType : IfcTypeProduct
	{
		public static readonly IfcSpaceHeaterType CorrectPredefinedType = new IfcSpaceHeaterType();
		protected IfcSpaceHeaterType() {}
	}
}
