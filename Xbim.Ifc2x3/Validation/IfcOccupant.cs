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
namespace Xbim.Ifc2x3.SharedFacilitiesElements
{
	public partial class IfcOccupant : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.SharedFacilitiesElements.IfcOccupant");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcOccupant clause) {
			var retVal = false;
			if (clause == Where.IfcOccupant.WR31) {
				try {
					retVal = !(PredefinedType == IfcOccupantTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcOccupant.WR31' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcOccupant.WR31))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcOccupant.WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcOccupant : IfcObject
	{
		public static readonly IfcOccupant WR31 = new IfcOccupant();
		protected IfcOccupant() {}
	}
}
