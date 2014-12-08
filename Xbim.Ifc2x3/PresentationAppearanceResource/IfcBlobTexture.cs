using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    /// <summary>
    /// An IfcBlobTexture provides a 2-dimensional distribution of the lighting parameters 
    /// of a surface onto which it is mapped. The texture itself is given as a single binary, 
    /// representing the content of a pixel format. The file format of the pixel file is given by 
    /// the RasterFormat attribute and allowable formats are guided by where rule WR41.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcBlobTexture : IfcSurfaceTexture
    {
        private IfcIdentifier _RasterFormat;

        /// <summary>
        /// The format of the RasterCode often using a compression. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcIdentifier RasterFormat
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RasterFormat;
            }
            set { this.SetModelValue(this, ref _RasterFormat, value, v => RasterFormat = v, "RasterFormat"); }
        }

        private IfcBoolean _RasterCode;

        /// <summary>
        /// Blob, given as a single binary, to capture the texture within one popular file (compression) format. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcBoolean RasterCode
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _RasterCode;
            }
            set { this.SetModelValue(this, ref _RasterCode, value, v => RasterCode = v, "RasterCode"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    base.IfcParse(propIndex, value);
                    break;
                case 4:
                    _RasterFormat = value.StringVal;
                    break;
                case 5:
                    _RasterCode = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            var formats = new []{"BMP", "JPG", "GIF", "PNG"};
            if (!formats.Any(f => f == RasterFormat))
                return "WR11: Currently the formats of bmp, jpg, gif and pgn, shall be supported. \n";
            else
                return "";
        }

    }
}
