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
namespace Xbim.Ifc4.RepresentationResource
{
	public partial class IfcProjectedCRS : IExpressValidatable
	{
		public enum IfcProjectedCRSClause
		{
			IsLengthUnit,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcProjectedCRSClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcProjectedCRSClause.IsLengthUnit:
						retVal = !(EXISTS(MapUnit)) || (MapUnit.UnitType == IfcUnitEnum.LENGTHUNIT);
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.RepresentationResource.IfcProjectedCRS");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProjectedCRS.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcProjectedCRSClause.IsLengthUnit))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProjectedCRS.IsLengthUnit", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
