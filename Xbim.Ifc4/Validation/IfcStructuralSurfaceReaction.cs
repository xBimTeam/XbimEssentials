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
namespace Xbim.Ifc4.StructuralAnalysisDomain
{
	public partial class IfcStructuralSurfaceReaction : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralSurfaceReaction");

		/// <summary>
		/// Tests the express where clause HasPredefinedType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasPredefinedType() {
			var retVal = false;
			try {
				retVal = (PredefinedType != IfcStructuralSurfaceActivityTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasPredefinedType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!HasPredefinedType())
				yield return new ValidationResult() { Item = this, IssueSource = "HasPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
