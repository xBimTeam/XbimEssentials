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
namespace Xbim.Ifc4.PropertyResource
{
	public partial class IfcPropertyDependencyRelationship : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PropertyResource.IfcPropertyDependencyRelationship");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPropertyDependencyRelationship clause) {
			var retVal = false;
			if (clause == Where.IfcPropertyDependencyRelationship.NoSelfReference) {
				try {
					retVal = !Object.ReferenceEquals(DependingProperty, DependantProperty);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPropertyDependencyRelationship.NoSelfReference' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcPropertyDependencyRelationship.NoSelfReference))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPropertyDependencyRelationship.NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPropertyDependencyRelationship
	{
		public static readonly IfcPropertyDependencyRelationship NoSelfReference = new IfcPropertyDependencyRelationship();
		protected IfcPropertyDependencyRelationship() {}
	}
}
