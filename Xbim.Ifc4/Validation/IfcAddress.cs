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
	public partial class IfcAddress : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcAddress clause) {
			var retVal = false;
			if (clause == Where.IfcAddress.WR1) {
				try {
					retVal = (!(EXISTS(Purpose))) || ((Purpose != IfcAddressTypeEnum.USERDEFINED) || ((Purpose == IfcAddressTypeEnum.USERDEFINED) && EXISTS(this.UserDefinedPurpose)));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ActorResource.IfcAddress");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcAddress.WR1' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcAddress.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAddress.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcAddress
	{
		public static readonly IfcAddress WR1 = new IfcAddress();
		protected IfcAddress() {}
	}
}
