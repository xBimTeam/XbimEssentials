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

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDimensionCalloutRelationship clause) {
			var retVal = false;
			if (clause == Where.IfcDimensionCalloutRelationship.WR11) {
				try {
					retVal = NewArray("primary", "secondary").Contains(this/* as IfcDraughtingCalloutRelationship*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCalloutRelationship");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCalloutRelationship.WR11' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcDimensionCalloutRelationship.WR12) {
				try {
					retVal = SIZEOF(TYPEOF(this/* as IfcDraughtingCalloutRelationship*/.RelatingDraughtingCallout) * NewArray("IFC2X3.IFCANGULARDIMENSION", "IFC2X3.IFCDIAMETERDIMENSION", "IFC2X3.IFCLINEARDIMENSION", "IFC2X3.IFCRADIUSDIMENSION")) == 1;
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCalloutRelationship");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCalloutRelationship.WR12' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcDimensionCalloutRelationship.WR13) {
				try {
					retVal = !(TYPEOF(this/* as IfcDraughtingCalloutRelationship*/.RelatedDraughtingCallout).Contains("IFC2X3.IFCDIMENSIONCURVEDIRECTEDCALLOUT"));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionCalloutRelationship");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionCalloutRelationship.WR13' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcDimensionCalloutRelationship.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCalloutRelationship.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcDimensionCalloutRelationship.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCalloutRelationship.WR12", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcDimensionCalloutRelationship.WR13))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionCalloutRelationship.WR13", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcDimensionCalloutRelationship
	{
		public static readonly IfcDimensionCalloutRelationship WR11 = new IfcDimensionCalloutRelationship();
		public static readonly IfcDimensionCalloutRelationship WR12 = new IfcDimensionCalloutRelationship();
		public static readonly IfcDimensionCalloutRelationship WR13 = new IfcDimensionCalloutRelationship();
		protected IfcDimensionCalloutRelationship() {}
	}
}
