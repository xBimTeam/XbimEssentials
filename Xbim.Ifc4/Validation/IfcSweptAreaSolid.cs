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
		/// Tests the express where clause SweptAreaType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SweptAreaType() {
			var retVal = false;
			try {
				retVal = SweptArea.ProfileType == IfcProfileTypeEnum.AREA;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SweptAreaType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!SweptAreaType())
				yield return new ValidationResult() { Item = this, IssueSource = "SweptAreaType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
