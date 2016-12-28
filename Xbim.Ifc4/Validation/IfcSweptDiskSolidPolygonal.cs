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
	public partial class IfcSweptDiskSolidPolygonal : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcSweptDiskSolidPolygonal");

		/// <summary>
		/// Tests the express where clause CorrectRadii
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectRadii() {
			var retVal = false;
			try {
				retVal = !(EXISTS(FilletRadius)) || (FilletRadius >= this/* as IfcSweptDiskSolid*/.Radius);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectRadii' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause DirectrixIsPolyline
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool DirectrixIsPolyline() {
			var retVal = false;
			try {
				retVal = TYPEOF(this/* as IfcSweptDiskSolid*/.Directrix).Contains("IFC4.IFCPOLYLINE");
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'DirectrixIsPolyline' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!CorrectRadii())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectRadii", IssueType = ValidationFlags.EntityWhereClauses };
			if (!DirectrixIsPolyline())
				yield return new ValidationResult() { Item = this, IssueSource = "DirectrixIsPolyline", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
