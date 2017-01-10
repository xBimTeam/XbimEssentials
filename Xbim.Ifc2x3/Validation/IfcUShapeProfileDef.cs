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
namespace Xbim.Ifc2x3.ProfileResource
{
	public partial class IfcUShapeProfileDef : IExpressValidatable
	{
		public enum IfcUShapeProfileDefClause
		{
			WR21,
			WR22,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcUShapeProfileDefClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcUShapeProfileDefClause.WR21:
						retVal = FlangeThickness < (Depth / 2);
						break;
					case IfcUShapeProfileDefClause.WR22:
						retVal = WebThickness < FlangeWidth;
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc2x3.ProfileResource.IfcUShapeProfileDef");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcUShapeProfileDef.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcUShapeProfileDefClause.WR21))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcUShapeProfileDef.WR21", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcUShapeProfileDefClause.WR22))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcUShapeProfileDef.WR22", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
