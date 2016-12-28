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
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcRepresentationMap : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcRepresentationMap");

		/// <summary>
		/// Tests the express where clause ApplicableMappedRepr
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ApplicableMappedRepr() {
			var retVal = false;
			try {
				retVal = TYPEOF(MappedRepresentation).Contains("IFC4.IFCSHAPEMODEL");
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ApplicableMappedRepr' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ApplicableMappedRepr())
				yield return new ValidationResult() { Item = this, IssueSource = "ApplicableMappedRepr", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
