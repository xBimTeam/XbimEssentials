using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc4.Interfaces;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.ActorResource
{
	public partial class IfcAddress : IExpressValidatable
	{
		public enum IfcAddressClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcAddressClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcAddressClause.WR1:
						retVal = (!(Functions.EXISTS(Purpose))) || ((Purpose != IfcAddressTypeEnum.USERDEFINED) || ((Purpose == IfcAddressTypeEnum.USERDEFINED) && Functions.EXISTS(this.UserDefinedPurpose)));
						break;
				}
			} catch (Exception  ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc4.ActorResource.IfcAddress>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcAddress.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcAddressClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcAddress.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
