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
namespace Xbim.Ifc4.Kernel
{
	public partial class IfcRelAssignsToControl : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.Kernel.IfcRelAssignsToControl");

		/// <summary>
		/// Tests the express where clause NoSelfReference
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool NoSelfReference() {
			var retVal = false;
			try {
				retVal = SIZEOF(this/* as IfcRelAssigns*/.RelatedObjects.Where(Temp => Object.ReferenceEquals(RelatingControl, Temp))) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'NoSelfReference' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!NoSelfReference())
				yield return new ValidationResult() { Item = this, IssueSource = "NoSelfReference", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
