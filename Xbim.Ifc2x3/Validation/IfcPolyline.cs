using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.GeometryResource
{
	public partial class IfcPolyline : IExpressValidatable
	{
		public enum IfcPolylineClause
		{
			WR41,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcPolylineClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcPolylineClause.WR41:
						retVal = Functions.SIZEOF(Points.Where(Temp => Temp.Dim != Points.ItemAt(0).Dim)) == 0;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.GeometryResource.IfcPolyline>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcPolyline.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcPolylineClause.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPolyline.WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
