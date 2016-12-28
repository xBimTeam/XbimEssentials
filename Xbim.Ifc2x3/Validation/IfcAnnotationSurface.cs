using System;
using log4net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
	public partial class IfcAnnotationSurface : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDefinitionResource.IfcAnnotationSurface");

		/// <summary>
		/// Tests the express where clause WR01
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR01() {
			var retVal = false;
			try {
				retVal = SIZEOF(NewArray("IFC2X3.IFCSURFACE", "IFC2X3.IFCSHELLBASEDSURFACEMODEL", "IFC2X3.IFCFACEBASEDSURFACEMODEL", "IFC2X3.IFCSOLIDMODEL", "IFC2X3.IFCBOOLEANRESULT", "IFC2X3.IFCCSGPRIMITIVE3D") * TYPEOF(Item)) >= 1;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR01' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!WR01())
				yield return new ValidationResult() { Item = this, IssueSource = "WR01", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
