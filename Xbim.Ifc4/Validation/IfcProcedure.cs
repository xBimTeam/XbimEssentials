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
namespace Xbim.Ifc4.ProcessExtension
{
	public partial class IfcProcedure : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProcessExtension.IfcProcedure");

		/// <summary>
		/// Tests the express where clause HasName
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool HasName() {
			var retVal = false;
			try {
				retVal = EXISTS(this/* as IfcRoot*/.Name);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'HasName' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause CorrectPredefinedType
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool CorrectPredefinedType() {
			var retVal = false;
			try {
				retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcProcedureTypeEnum.USERDEFINED) || ((PredefinedType == IfcProcedureTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'CorrectPredefinedType' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public new IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!HasName())
				yield return new ValidationResult() { Item = this, IssueSource = "HasName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!CorrectPredefinedType())
				yield return new ValidationResult() { Item = this, IssueSource = "CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
