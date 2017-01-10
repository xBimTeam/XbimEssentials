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
namespace Xbim.Ifc4.ArchitectureDomain
{
	public partial class IfcDoorPanelProperties : IExpressValidatable
	{

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcDoorPanelProperties clause) {
			var retVal = false;
			if (clause == Where.IfcDoorPanelProperties.ApplicableToType) {
				try {
					retVal = (EXISTS(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0))) && ((TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC4.IFCDOORTYPE")) || (TYPEOF(this/* as IfcPropertySetDefinition*/.DefinesType.ItemAt(0)).Contains("IFC4.IFCDOORSTYLE")));
				} catch (Exception ex) {
					ILog Log = LogManager.GetLogger("Xbim.Ifc4.ArchitectureDomain.IfcDoorPanelProperties");
					Log.Error(string.Format("Exception thrown evaluating where-clause 'IfcDoorPanelProperties.ApplicableToType' for #{0}.",EntityLabel), ex);
				}
				return retVal;
			}
			throw new ArgumentException(string.Format("Invalid clause specifier: '{0}'", clause));
		}

		public virtual IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcDoorPanelProperties.ApplicableToType))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcDoorPanelProperties.ApplicableToType", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcDoorPanelProperties
	{
		public static readonly IfcDoorPanelProperties ApplicableToType = new IfcDoorPanelProperties();
		protected IfcDoorPanelProperties() {}
	}
}
