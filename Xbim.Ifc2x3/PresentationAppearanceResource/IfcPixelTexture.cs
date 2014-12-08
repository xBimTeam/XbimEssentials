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
    ///  An IfcPixelTexture provides a 2D image-based texture map as an explicit array of pixel values (image field). In contrary to the IfcImageTexture the IfcPixelTexture holds a 2 dimensional list of pixel color (and opacity) directly, instead of referencing to an URL.
    /// 
    /// The following additional definitions from ISO/IEC FCD 19775:200x, the Extensible 3D (X3D) specification, apply:
    /// 
    ///     The PixelTexture node defines a 2D image-based texture map as an explicit array of pixel values (image field) and parameters controlling tiling repetition of the texture onto geometry. Texture maps are defined in a 2D coordinate system (s, t) that ranges from 0.0 to 1.0 in both directions. The bottom edge of the pixel image corresponds to the S-axis of the texture map, and left edge of the pixel image corresponds to the T-axis of the texture map. The lower-left pixel of the pixel image corresponds to s=0.0, t=0.0, and the top-right pixel of the image corresponds to s = 1.0, t = 1.0.
    /// 
    /// The following general recommendations for explicit image array format support from ISO/IEC FCD 19775:200x, the Extensible 3D (X3D) specification, also apply:
    /// 
    ///     The Image field specifies a single uncompressed 2-dimensional pixel image. Image fields contain three integers representing the width, height and number of components in the image, followed by width×height hexadecimal values representing the pixels in the image. Pixel values are limited to 256 levels of intensity (i.e., 0x00-0xFF hexadecimal).
    /// 
    ///         A one-component image specifies one-byte hexadecimal value representing the intensity of the image. For example, 0xFF is full intensity in hexadecimal (255 in decimal), 0x00 is no intensity (0 in decimal).
    ///         A two-component image specifies the intensity in the first (high) byte and the alpha opacity in the second (low) byte.
    ///         Pixels in a three-component image specify the red component in the first (high) byte, followed by the green and blue components (e.g., 0xFF0000 is red, 0x00FF00 is green, 0x0000FF is blue).
    ///         Four-component images specify the alpha opacity byte after red/green/blue (e.g., 0x0000FF80 is semi-transparent blue). A value of 00 is completely transparent, FF is completely opaque, 80 is semi-transparent.
    /// 
    ///     Note that alpha equals (1.0 -transparency), if alpha and transparency each range from 0.0 to 1.0.
    /// 
    ///     Each pixel is read as a single unsigned number. For example, a 3-component pixel with value 0x0000FF may also be written as 0xFF (hexadecimal). Pixels are specified from left to right, bottom to top. The first hexadecimal value is the lower left pixel and the last value is the upper right pixel.
    /// 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcPixelTexture : IfcSurfaceTexture
    {
        public IfcPixelTexture()
        {
            _Pixel = new XbimSet<long>(this);
        }

        private IfcInteger _Width;

        /// <summary>
        ///  The number of pixels in width (S) direction. 
        /// </summary>
        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcInteger Width
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Width;
            }
            set { this.SetModelValue(this, ref _Width, value, v => Width = v, "Width"); }
        }

        private IfcInteger _Height;

        /// <summary>
        /// The number of pixels in height (T) direction. 
        /// </summary>
        [IfcAttribute(6, IfcAttributeState.Mandatory)]
        public IfcInteger Height
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Height;
            }
            set { this.SetModelValue(this, ref _Height, value, v => Height = v, "Height"); }
        }

        private IfcInteger _ColourComponents;

        /// <summary>
        /// Indication whether the pixel values contain a 1, 2, 3, or 4 colour component. 
        /// </summary>
        [IfcAttribute(7, IfcAttributeState.Mandatory)]
        public IfcInteger ColourComponents
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _ColourComponents;
            }
            set { this.SetModelValue(this, ref _ColourComponents, value, v => ColourComponents = v, "ColourComponents"); }
        }

        private XbimSet<long> _Pixel;

        //TODO: Check if this is serializer properly as a hexadecimal values
        /// <summary>
        /// Flat list of hexadecimal values, each describing one pixel by 1, 2, 3, or 4 components. 
        /// </summary>
        [IfcAttribute(8, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.None, 1)]
        public XbimSet<long> Pixel
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Pixel;
            }
            set { this.SetModelValue(this, ref _Pixel, value, v => Pixel = v, "Pixel"); }
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
                    _Width = value.IntegerVal;
                    break;
                case 5:
                    _Height = value.IntegerVal;
                    break;
                case 6:
                    _ColourComponents = value.IntegerVal;
                    break;
                case 7:
                    _Pixel.Add(value.HexadecimalVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            var result = "";
            if (Width < 1)
                result += "WR21: The minimum number of pixel in width (S coordinate) direction shall be 1. \n";

            if (Height < 1)
                result += "WR22: The minimum number of pixel in height (T coordinate) direction shall be 1. \n";

            if (ColourComponents <= 1 || ColourComponents >= 4)
                result += "WR23: The number of color components shall be either 1, 2, 3, or 4. \n";

            if (Pixel.Count != (Width * Height))
                result += "WR24: The list of pixel shall have exactly width*height members. \n";

            return result;
        }
    }
}
