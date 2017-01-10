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
		public enum IfcRepresentationMapClause
		{
			ApplicableMappedRepr,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcRepresentationMapClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcRepresentationMapClause.ApplicableMappedRepr:
						retVal = TYPEOF(MappedRepresentation).Contains("IFC4.IFCSHAPEMODEL");
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.GeometryResource.IfcRepresentationMap");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcRepresentationMap.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcRepresentationMapClause.ApplicableMappedRepr))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcRepresentationMap.ApplicableMappedRepr", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
