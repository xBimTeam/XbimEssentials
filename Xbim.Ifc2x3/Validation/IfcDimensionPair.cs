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
	public partial class IfcDimensionPair : IExpressValidatable
	{
		public enum IfcDimensionPairClause
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
		public bool ValidateClause(IfcDimensionPairClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcDimensionPairClause.WR11:
						retVal = NewArray("chained", "parallel").Contains(this.Name);
						break;
					case IfcDimensionPairClause.WR12:
						retVal = SIZEOF(TYPEOF(this.RelatingDraughtingCallout) * NewArray("IFC2X3.IFCANGULARDIMENSION", "IFC2X3.IFCDIAMETERDIMENSION", "IFC2X3.IFCLINEARDIMENSION", "IFC2X3.IFCRADIUSDIMENSION")) == 1;
						break;
					case IfcDimensionPairClause.WR13:
						retVal = SIZEOF(TYPEOF(this.RelatedDraughtingCallout) * NewArray("IFC2X3.IFCANGULARDIMENSION", "IFC2X3.IFCDIAMETERDIMENSION", "IFC2X3.IFCLINEARDIMENSION", "IFC2X3.IFCRADIUSDIMENSION")) == 1;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.PresentationDimensioningResource.IfcDimensionPair");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDimensionPair.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcDimensionPairClause.WR11))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionPair.WR11", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDimensionPairClause.WR12))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionPair.WR12", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcDimensionPairClause.WR13))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDimensionPair.WR13", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
