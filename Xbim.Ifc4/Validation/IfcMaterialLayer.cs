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
namespace Xbim.Ifc4.MaterialResource
{
	public partial class IfcMaterialLayer : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.MaterialResource.IfcMaterialLayer");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcMaterialLayer clause) {
			var retVal = false;
			if (clause == Where.IfcMaterialLayer.NormalizedPriority) {
				try {
					retVal = !(EXISTS(Priority)) || ((0 <= Priority) && (Priority <= 100) );
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcMaterialLayer.NormalizedPriority' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcMaterialLayer.NormalizedPriority))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMaterialLayer.NormalizedPriority", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcMaterialLayer
	{
		public static readonly IfcMaterialLayer NormalizedPriority = new IfcMaterialLayer();
		protected IfcMaterialLayer() {}
	}
}
