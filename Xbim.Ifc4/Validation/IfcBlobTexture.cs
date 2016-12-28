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
		/// Tests the express where clause SupportedRasterFormat
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool SupportedRasterFormat() {
			var retVal = false;
			try {
				retVal = NewArray("BMP", "JPG", "GIF", "PNG").Contains(this.RasterFormat);
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'SupportedRasterFormat' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		/// <summary>
		/// Tests the express where clause RasterCodeByteStream
		/// </summary>
		/// <returns>true if the clause is satisfied.</returns>
		public bool RasterCodeByteStream() {
			var retVal = false;
			try {
				retVal = BLENGTH(RasterCode) % 8 == 0;
			} catch (Exception ex) {
				Log.Error($"Exception thrown evaluating where-clause 'RasterCodeByteStream' for #{EntityLabel}.", ex);
			}
			return retVal;
		}

		public IEnumerable<ValidationResult> Validate()
		{
			if (!SupportedRasterFormat())
				yield return new ValidationResult() { Item = this, IssueSource = "SupportedRasterFormat", IssueType = ValidationFlags.EntityWhereClauses };
			if (!RasterCodeByteStream())
				yield return new ValidationResult() { Item = this, IssueSource = "RasterCodeByteStream", IssueType = ValidationFlags.EntityWhereClauses };
		}
	}
}
