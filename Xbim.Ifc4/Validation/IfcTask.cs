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
	public partial class IfcTask : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTask clause) {
			var retVal = false;
			if (clause == Where.IfcTask.HasName) {
				try {
					retVal = EXISTS(this/* as IfcRoot*/.Name);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProcessExtension.IfcTask");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTask.HasName' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			if (clause == Where.IfcTask.CorrectPredefinedType) {
				try {
					retVal = !(EXISTS(PredefinedType)) || (PredefinedType != IfcTaskTypeEnum.USERDEFINED) || ((PredefinedType == IfcTaskTypeEnum.USERDEFINED) && EXISTS(this/* as IfcObject*/.ObjectType));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ProcessExtension.IfcTask");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTask.CorrectPredefinedType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcObject)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcTask.HasName))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.HasName", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcTask.CorrectPredefinedType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTask.CorrectPredefinedType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcTask : IfcObject
	{
		public static readonly IfcTask HasName = new IfcTask();
		public static readonly IfcTask CorrectPredefinedType = new IfcTask();
		protected IfcTask() {}
	}
}
