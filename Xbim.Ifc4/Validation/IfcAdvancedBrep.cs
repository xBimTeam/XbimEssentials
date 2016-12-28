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
		/// Tests the express where clause HasAdvancedFaces
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasAdvancedFaces() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcManifoldSolidBrep*/.Outer.CfsFaces.Where(Afs => (!(TYPEOF(Afs).Contains("IFC4.IFCADVANCEDFACE"))))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasAdvancedFaces' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!HasAdvancedFaces())
				yield return new ValidationResult() { Item = this, IssueSource = "HasAdvancedFaces", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
