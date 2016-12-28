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
	public partial class IfcRevolvedAreaSolidTapered : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcRevolvedAreaSolidTapered");

		/// <summary>
		/// Tests the express where clause CorrectProfileAssignment
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectProfileAssignment() {
			var retVal = false;
			try {
				retVal = IfcTaperedSweptAreaProfiles(this/* as IfcSweptAreaSolid*/.SweptArea, this.EndSweptArea);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectProfileAssignment' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!CorrectProfileAssignment())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectProfileAssignment", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
