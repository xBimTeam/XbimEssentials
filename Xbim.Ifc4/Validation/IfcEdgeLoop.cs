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
namespace Xbim.Ifc4.TopologyResource
{
	public partial class IfcEdgeLoop : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.TopologyResource.IfcEdgeLoop");

		/// <summary>
		/// Tests the express where clause IsClosed
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool IsClosed() {
			var retVal = false;
			try {
				retVal = Object.ReferenceEquals((EdgeList.ToArray()[0].EdgeStart), (EdgeList.ToArray()[Ne-1].EdgeEnd));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'IsClosed' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause IsContinuous
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool IsContinuous() {
			var retVal = false;
			try {
				retVal = IfcLoopHeadToTail(this);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'IsContinuous' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!IsClosed())
				yield return new ValidationResult() { Item = this, IssueSource = "IsClosed", IssueType = ValidationFlags.EntityWhereClauses };
			if (!IsContinuous())
				yield return new ValidationResult() { Item = this, IssueSource = "IsContinuous", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
