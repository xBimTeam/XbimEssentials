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
namespace Xbim.Ifc4.ActorResource
{
	public partial class IfcPostalAddress : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ActorResource.IfcPostalAddress");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcPostalAddress clause) {
			var retVal = false;
			if (clause == Where.IfcPostalAddress.WR1) {
				try {
					retVal = EXISTS(InternalLocation) || EXISTS(AddressLines) || EXISTS(PostalBox) || EXISTS(PostalCode) || EXISTS(Town) || EXISTS(Region) || EXISTS(Country);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcPostalAddress.WR1' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcAddress)clause);
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcPostalAddress.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcPostalAddress.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcPostalAddress : IfcAddress
	{
		public new static readonly IfcPostalAddress WR1 = new IfcPostalAddress();
		protected IfcPostalAddress() {}
	}
}
