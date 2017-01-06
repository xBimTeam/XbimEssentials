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
namespace Xbim.Ifc4.StructuralLoadResource
{
	public partial class IfcSurfaceReinforcementArea : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralLoadResource.IfcSurfaceReinforcementArea");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcSurfaceReinforcementArea clause) {
			var retVal = false;
			if (clause == Where.IfcSurfaceReinforcementArea.SurfaceAndOrShearAreaSpecified) {
				try {
					retVal = EXISTS(SurfaceReinforcement1) || EXISTS(SurfaceReinforcement2) || EXISTS(ShearReinforcement);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceReinforcementArea.SurfaceAndOrShearAreaSpecified' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceReinforcementArea.NonnegativeArea1) {
				try {
					retVal = (!EXISTS(SurfaceReinforcement1)) || ((SurfaceReinforcement1.ToArray()[0] >= 0) && (SurfaceReinforcement1.ToArray()[1] >= 0) && ((SIZEOF(SurfaceReinforcement1) == 1) || (SurfaceReinforcement1.ToArray()[0] >= 0)));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceReinforcementArea.NonnegativeArea1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceReinforcementArea.NonnegativeArea2) {
				try {
					retVal = (!EXISTS(SurfaceReinforcement2)) || ((SurfaceReinforcement2.ToArray()[0] >= 0) && (SurfaceReinforcement2.ToArray()[1] >= 0) && ((SIZEOF(SurfaceReinforcement2) == 1) || (SurfaceReinforcement2.ToArray()[0] >= 0)));
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceReinforcementArea.NonnegativeArea2' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcSurfaceReinforcementArea.NonnegativeArea3) {
				try {
					retVal = (!EXISTS(ShearReinforcement)) || (ShearReinforcement >= 0);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcSurfaceReinforcementArea.NonnegativeArea3' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcSurfaceReinforcementArea.SurfaceAndOrShearAreaSpecified))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceReinforcementArea.SurfaceAndOrShearAreaSpecified", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceReinforcementArea.NonnegativeArea1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceReinforcementArea.NonnegativeArea1", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceReinforcementArea.NonnegativeArea2))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceReinforcementArea.NonnegativeArea2", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcSurfaceReinforcementArea.NonnegativeArea3))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcSurfaceReinforcementArea.NonnegativeArea3", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcSurfaceReinforcementArea
	{
		public static readonly IfcSurfaceReinforcementArea SurfaceAndOrShearAreaSpecified = new IfcSurfaceReinforcementArea();
		public static readonly IfcSurfaceReinforcementArea NonnegativeArea1 = new IfcSurfaceReinforcementArea();
		public static readonly IfcSurfaceReinforcementArea NonnegativeArea2 = new IfcSurfaceReinforcementArea();
		public static readonly IfcSurfaceReinforcementArea NonnegativeArea3 = new IfcSurfaceReinforcementArea();
		protected IfcSurfaceReinforcementArea() {}
	}
}
