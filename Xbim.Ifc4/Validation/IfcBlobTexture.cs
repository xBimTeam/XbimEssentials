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
	public partial class IfcBlobTexture : IExpressValidatable
	{
		private static readonly ILog Log = LogManager.GetLogger("Xbim.Ifc4.PresentationAppearanceResource.IfcBlobTexture");

		/// <summary>
		/// Tests the express where-clause specified in param 'clause'
		/// </summary>
		/// <param name="clause">The express clause to test</param>
		/// <returns>true if the clause is satisfied.</returns>
		public bool ValidateClause(Where.IfcBlobTexture clause) {
			var retVal = false;
			if (clause == Where.IfcBlobTexture.SupportedRasterFormat) {
				try {
					retVal = NewArray("BMP", "JPG", "GIF", "PNG").Contains(this.RasterFormat);
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBlobTexture.SupportedRasterFormat' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			if (clause == Where.IfcBlobTexture.RasterCodeByteStream) {
				try {
					retVal = BLENGTH(RasterCode) % 8 == 0;
				} catch (Exception ex) {
					Log.Error($"Exception thrown evaluating where-clause 'IfcBlobTexture.RasterCodeByteStream' for #{EntityLabel}.", ex);
				}
				return retVal;
			}
			throw new ArgumentException($"Invalid clause specifier: '{clause}'", nameof(clause));
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!ValidateClause(Where.IfcBlobTexture.SupportedRasterFormat))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBlobTexture.SupportedRasterFormat", IssueType = ValidationFlags.EntityWhereClauses };
			if (!ValidateClause(Where.IfcBlobTexture.RasterCodeByteStream))
				yield return new ValidationResult() { Item = this, IssueSource = "IfcBlobTexture.RasterCodeByteStream", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Xbim.Ifc4.Where
{
	public class IfcBlobTexture
	{
		public static readonly IfcBlobTexture SupportedRasterFormat = new IfcBlobTexture();
		public static readonly IfcBlobTexture RasterCodeByteStream = new IfcBlobTexture();
		protected IfcBlobTexture() {}
	}
}
