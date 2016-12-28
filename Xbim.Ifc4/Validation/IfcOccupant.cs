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
namespace Xbim.Ifc4.SharedFacilitiesElements
{
	public partial class IfcOccupant : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.SharedFacilitiesElements.IfcOccupant");

		/// <summary>
		/// Tests the express where clause WR31
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool WR31() {
			var retVal = false;
			try {
				retVal = !(PredefinedType == IfcOccupantTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'WR31' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!WR31())
				yield return new ValidationResult() { Item = this, IssueSource = "WR31", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
