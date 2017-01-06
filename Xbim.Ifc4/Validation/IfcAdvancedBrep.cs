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
	public partial class IfcAdvancedBrep : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcAdvancedBrep");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAdvancedBrep clause) {
			var retVal = false;
			if (clause == Where.IfcAdvancedBrep.HasAdvancedFaces) {
				try {
					retVal = SIZEOF(this/* as IfcManifoldSolidBrep*/.Outer.CfsFaces.Where(Afs => (!(TYPEOF(Afs).Contains("IFC4.IFCADVANCEDFACE"))))) == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcAdvancedBrep.HasAdvancedFaces' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcAdvancedBrep.HasAdvancedFaces))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAdvancedBrep.HasAdvancedFaces", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAdvancedBrep
	{
		public static readonly IfcAdvancedBrep HasAdvancedFaces = new IfcAdvancedBrep();
		protected IfcAdvancedBrep() {}
	}
}
