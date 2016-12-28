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
namespace Xbim.Ifc4.GeometricModelResource
{
	public partial class IfcGeometricSet : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcGeometricSet");

		/// <summary>
		/// Tests the express where clause ConsistentDim
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ConsistentDim() {
			var retVal = false;
			try {
				retVal = SIZEOF(Elements.Where(Temp => Temp.Dim != Elements.ToArray()[0].Dim)) == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'ConsistentDim' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ConsistentDim())
				yield return new ValidationResult() { Item = this, IssueSource = "ConsistentDim", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
