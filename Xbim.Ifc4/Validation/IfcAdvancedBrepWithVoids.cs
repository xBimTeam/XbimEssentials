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
	public partial class IfcAdvancedBrepWithVoids : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcAdvancedBrepWithVoids");

		/// <summary>
		/// Tests the express where clause VoidsHaveAdvancedFaces
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool VoidsHaveAdvancedFaces() {
			var retVal = false;
			try {
				retVal = SIZEOF(Voids.Where(Vsh => SIZEOF(Vsh.CfsFaces.Where(Afs => (!(TYPEOF(Afs).Contains("IFC4.IFCADVANCEDFACE"))))) == 0)) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'VoidsHaveAdvancedFaces' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!VoidsHaveAdvancedFaces())
				yield return new ValidationResult() { Item = this, IssueSource = "VoidsHaveAdvancedFaces", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
