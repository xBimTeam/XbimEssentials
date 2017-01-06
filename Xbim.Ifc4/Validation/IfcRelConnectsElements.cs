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
	public partial class IfcRelConnectsElements : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProductExtension.IfcRelConnectsElements");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcRelConnectsElements clause) {
			var retVal = false;
			if (clause == Where.IfcRelConnectsElements.NoSelfReference) {
				try {
					retVal = !Object.ReferenceEquals(RelatingElement, RelatedElement);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcRelConnectsElements.NoSelfReference' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcRelConnectsElements.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRelConnectsElements.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcRelConnectsElements
	{
		public static readonly IfcRelConnectsElements NoSelfReference = new IfcRelConnectsElements();
		protected IfcRelConnectsElements() {}
	}
}
