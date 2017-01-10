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
namespace Xbim.Ifc4.PresentationAppearanceResource
{
	public partial struct IfcTextDecoration : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTextDecoration clause) {
			var retVal = false;
			if (clause == Where.IfcTextDecoration.WR1) {
				try {
					retVal = NewArray("none", "underline", "overline", "line-through", "blink").Contains(this);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcTextDecoration");
					Log.Error("Exception thrown evaluating where-clause 'IfcTextDecoration.WR1'.", ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public  IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcTextDecoration.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextDecoration.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcTextDecoration
	{
		public static readonly IfcTextDecoration WR1 = new IfcTextDecoration();
		protected IfcTextDecoration() {}
	}
}
