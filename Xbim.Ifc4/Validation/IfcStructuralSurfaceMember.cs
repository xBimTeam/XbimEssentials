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
	public partial class IfcStructuralSurfaceMember : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.StructuralAnalysisDomain.IfcStructuralSurfaceMember");

		/// <summary>
		/// Tests the express where clause HasObjectType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasObjectType() {
			var retVal = false;
			try {
				retVal = (PredefinedType != IfcStructuralSurfaceMemberTypeEnum.USERDEFINED) || EXISTS(this/* as IfcObject*/.ObjectType);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasObjectType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!HasObjectType())
				yield return new ValidationResult() { Item = this, IssueSource = "HasObjectType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
