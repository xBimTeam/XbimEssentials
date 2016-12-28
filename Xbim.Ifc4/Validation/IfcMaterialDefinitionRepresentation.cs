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
	public partial class IfcMaterialDefinitionRepresentation : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcMaterialDefinitionRepresentation");

		/// <summary>
		/// Tests the express where clause OnlyStyledRepresentations
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool OnlyStyledRepresentations() {
			var retVal = false;
			try {
				retVal = SIZEOF(Representations.Where(temp => (!(TYPEOF(temp).Contains("IFC4.IFCSTYLEDREPRESENTATION"))))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'OnlyStyledRepresentations' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!OnlyStyledRepresentations())
				yield return new ValidationResult() { Item = this, IssueSource = "OnlyStyledRepresentations", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
