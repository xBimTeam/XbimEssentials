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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcStyledRepresentation : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcStyledRepresentation");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcStyledRepresentation clause) {
			var retVal = false;
			if (clause == Where.IfcStyledRepresentation.OnlyStyledItems) {
				try {
					retVal = SIZEOF(this/* as IfcRepresentation*/.Items.Where(temp => (!(TYPEOF(temp).Contains("IFC4.IFCSTYLEDITEM"))))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcStyledRepresentation.OnlyStyledItems' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcStyledRepresentation.OnlyStyledItems))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcStyledRepresentation.OnlyStyledItems", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcStyledRepresentation
	{
		public static readonly IfcStyledRepresentation OnlyStyledItems = new IfcStyledRepresentation();
		protected IfcStyledRepresentation() {}
	}
}
