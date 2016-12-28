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
	public partial class IfcProductDefinitionShape : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcProductDefinitionShape");

		/// <summary>
		/// Tests the express where clause OnlyShapeModel
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool OnlyShapeModel() {
			var retVal = false;
			try {
				retVal = SIZEOF(Representations.Where(temp => (!(TYPEOF(temp).Contains("IFC4.IFCSHAPEMODEL"))))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'OnlyShapeModel' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!OnlyShapeModel())
				yield return new ValidationResult() { Item = this, IssueSource = "OnlyShapeModel", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
