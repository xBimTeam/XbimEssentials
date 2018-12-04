using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.GeometryResource
{
	public partial class IfcCartesianPoint : IExpressValidatable
	{
		public enum IfcCartesianPointClause
		{
			CP2Dor3D,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcCartesianPointClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcCartesianPointClause.CP2Dor3D:
						retVal = Functions.HIINDEX(Coordinates) >= 2;
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.GeometryResource.IfcCartesianPoint>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcCartesianPoint.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcCartesianPointClause.CP2Dor3D))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcCartesianPoint.CP2Dor3D", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
