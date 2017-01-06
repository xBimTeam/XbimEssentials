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
	public partial class IfcTextureMap : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDefinitionResource.IfcTextureMap");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTextureMap clause) {
			var retVal = false;
			if (clause == Where.IfcTextureMap.WR11) {
				try {
					retVal = SIZEOF(NewArray("IFC2X3.IFCSHELLBASEDSURFACEMODEL", "IFC2X3.IFCFACEBASEDSURFACEMODEL", "IFC2X3.IFCFACETEDBREP", "IFC2X3.IFCFACETEDBREPWITHVOIDS") * TYPEOF(this/* as IfcTextureCoordinate*/.AnnotatedSurface.ToArray()[0].Item)) >= 1;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcTextureMap.WR11' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTextureMap.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextureMap.WR11", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcTextureMap
	{
		public static readonly IfcTextureMap WR11 = new IfcTextureMap();
		protected IfcTextureMap() {}
	}
}
