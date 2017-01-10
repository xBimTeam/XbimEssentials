using System;
using log4net;
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
using static Xbim.Ifc2x3.Functions;
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Kernel
{
	public partial class IfcTypeProduct : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcTypeProduct clause) {
			var retVal = false;
			if (clause == Where.IfcTypeProduct.WR41) {
				try {
					retVal = !(EXISTS(this/* as IfcTypeObject*/.ObjectTypeOf.ItemAt(0))) || (SIZEOF(this/* as IfcTypeObject*/.ObjectTypeOf.ItemAt(0).RelatedObjects.Where(temp => !(TYPEOF(temp).Contains("IFC2X3.IFCPRODUCT")))) == 0);
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc2x3.Kernel.IfcTypeProduct");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcTypeProduct.WR41' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			return base.ValidateClause((Where.IfcTypeObject)clause);
		}

		public override IEnumerable<ValidationResult> Validate()
		{
			foreach (var value in base.Validate())
			{
				yield return value;
			}
			if (!ValidateClause(Where.IfcTypeProduct.WR41))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcTypeProduct.WR41", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc2x3.Where
{
	public class IfcTypeProduct : IfcTypeObject
	{
		public static readonly IfcTypeProduct WR41 = new IfcTypeProduct();
		protected IfcTypeProduct() {}
	}
}
