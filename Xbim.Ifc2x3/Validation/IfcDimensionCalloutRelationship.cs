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
namespace Xbim.Ifc2x3.PresentationDimensioningResource
{
	public partial class IfcDimensionCalloutRelationship : IExpressValidatable
	{
		public enum IfcDimensionCalloutRelationshipClause
		{
			WR11,
			WR12,
			WR13,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcDimensionCalloutRelationshipClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDimensionCalloutRelationshipClause.WR11:
						retVal = NewArray("primary", "secondary").Contains(this/* as IfcDraughtingCalloutRelationship*/.Name);
						break;
					case IfcDimensionCalloutRelationshipClause.WR12:
						retVal = SIZEOF(TYPEOF(this/* as IfcDraughtingCalloutRelationship*/.RelatingDraughtingCallout) * NewArray("IFC2X3.IFCANGULARDIMENSION", "IFC2X3.IFCDIAMETERDIMENSION", "IFC2X3.IFCLINEARDIMENSION", "IFC2X3.IFCRADIUSDIMENSION")) == 1;
						break;
					case IfcDimensionCalloutRelationshipClause.WR13:
						retVal = !(TYPEOF(this/* as IfcDraughtingCalloutRelationship*/.RelatedDraughtingCallout).Contains("IFC2X3.IFCDIMENSIONCURVEDIRECTEDCALLOUT"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCalloutRelationship");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCalloutRelationship.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDimensionCalloutRelationshipClause.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCalloutRelationship.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDimensionCalloutRelationshipClause.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCalloutRelationship.WR12", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDimensionCalloutRelationshipClause.WR13))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCalloutRelationship.WR13", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
