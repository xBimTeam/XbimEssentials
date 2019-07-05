using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
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
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.DateTimeResource
{
	public partial struct IfcMinuteInHour : IExpressValidatable
	{
		public enum IfcMinuteInHourClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcMinuteInHourClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcMinuteInHourClause.WR1:
						retVal = ((0 <= this) && (this <= 59) );
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.DateTimeResource.IfcMinuteInHour>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcMinuteInHour.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcMinuteInHourClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMinuteInHour.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
