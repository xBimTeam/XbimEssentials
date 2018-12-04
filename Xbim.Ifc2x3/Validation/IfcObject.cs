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
namespace Xbim.Ifc2x3.Kernel
{
	public partial class IfcObject : IExpressValidatable
	{
		public enum IfcObjectClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcObjectClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcObjectClause.WR1:
						retVal = Functions.SIZEOF(IsDefinedBy.Where(temp => Functions.TYPEOF(temp).Contains("IFC2X3.IFCRELDEFINESBYTYPE"))) <= 1;
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.Kernel.IfcObject>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcObject.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcObjectClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcObject.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
