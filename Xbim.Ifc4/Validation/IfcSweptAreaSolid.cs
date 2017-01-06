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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcSweptAreaSolid : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcSweptAreaSolid");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSweptAreaSolid clause) {
			var retVal = false;
			if (clause == Where.IfcSweptAreaSolid.SweptAreaType) {
				try {
					retVal = SweptArea.ProfileType == IfcProfileTypeEnum.AREA;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSweptAreaSolid.SweptAreaType' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSweptAreaSolid.SweptAreaType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSweptAreaSolid.SweptAreaType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSweptAreaSolid
	{
		public static readonly IfcSweptAreaSolid SweptAreaType = new IfcSweptAreaSolid();
		protected IfcSweptAreaSolid() {}
	}
}
