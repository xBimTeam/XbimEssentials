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
namespace Xbim.Ifc4.ElectricalDomain
{
	public partial class IfcMotorConnection : IExpressValidatable
	{
		public enum IfcMotorConnectionClause
		{
			CorrectPredefinedType,
			CorrectTypeAssigned,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcMotorConnectionClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcMotorConnectionClause.CorrectPredefinedType:
						retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcMotorConnectionTypeEnum.USERDEFINED) || ((PredefinedType == IfcMotorConnectionTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
						break;
					case IfcMotorConnectionClause.CorrectTypeAssigned:
						retVal = (SIZEOF(IsTypedBy) == 0) || (TYPEOF(this/* as IfcObject*/.IsTypedBy.ItemAt(0).RelatingType).Contains("IFC4.IFCMOTORCONNECTIONTYPE"));
						break;
				}
			} catch (Exception ex) {
				var Log = LogManager.GetLogger("Xbim.Ifc4.ElectricalDomain.IfcMotorConnection");
				Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcMotorConnection.{0}' for #{1}.", clause,EntityLabel), ex);
			}
			return retVal;
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(IfcMotorConnectionClause.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMotorConnection.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(IfcMotorConnectionClause.CorrectTypeAssigned))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcMotorConnection.CorrectTypeAssigned", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
