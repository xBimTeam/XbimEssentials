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
		public enum IfcProcedureClause
		{
			HasName,
			CorrectPredefinedType,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcProcedureClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcProcedureClause.HasName:
						retVal = EXISTS(this/* as IfcRoot*/.Name);
						break;
					case IfcProcedureClause.CorrectPredefinedType:
						retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcProcedureTypeEnum.USERDEFINED) || ((PredefinedType == IfcProcedureTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ProcessExtension.IfcProcedure");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcProcedure.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcProcedureClause.HasName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.HasName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcProcedureClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcProcedure.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
