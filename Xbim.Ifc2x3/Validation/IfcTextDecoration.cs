using System;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.ProfilePropertyResource;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial struct IfcTextDecoration : IExpressValidatable
	{
		public enum IfcTextDecorationClause
		{
			WR1,
		}

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(IfcTextDecorationClause clause) {
			var retVal = false;
			try
			{
				switch (clause)
				{
					case IfcTextDecorationClause.WR1:
						retVal = Functions.NewArray("none", "underline", "overline", "line-through", "blink").Contains(this);
						break;
				}
			} catch (Exception ex) {
				var log = Validation.ValidationLogging.CreateLogger<Xbim.Ifc2x3.PresentationAppearanceResource.IfcTextDecoration>();
				log?.LogError(string.Format("Exception thrown evaluating where-clause 'IfcTextDecoration.{0}'.", clause), ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(IfcTextDecorationClause.WR1))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTextDecoration.WR1", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
