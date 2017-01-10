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
	public partial class IfcExtrudedAreaSolidTapered : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcExtrudedAreaSolidTapered clause) {
			var retVal = false;
			if (clause == Where.IfcExtrudedAreaSolidTapered.CorrectProfileAssignment) {
				try {
					retVal = IfcTaperedSweptAreaProfiles(this/* as IfcSweptAreaSolid*/.SweptArea, this.EndSweptArea);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.GeometricModelResource.IfcExtrudedAreaSolidTapered");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcExtrudedAreaSolidTapered.CorrectProfileAssignment' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcExtrudedAreaSolid)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcExtrudedAreaSolidTapered.CorrectProfileAssignment))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcExtrudedAreaSolidTapered.CorrectProfileAssignment", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcExtrudedAreaSolidTapered : IfcExtrudedAreaSolid
	{
		public static readonly IfcExtrudedAreaSolidTapered CorrectProfileAssignment = new IfcExtrudedAreaSolidTapered();
		protected IfcExtrudedAreaSolidTapered() {}
	}
}
