using System;
using System.Collections.Generic;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4
{
  
    public class SurfaceStyling
    {
        public static SurfaceStyling Empty { get; private set; } 
        public SurfaceStyle FrontFaceStyle;
        public SurfaceStyle BackFaceStyle;

        static SurfaceStyling()
        {
            Empty = new SurfaceStyling();
        }
        public SurfaceStyling(SurfaceStyle frontFaceStyle, SurfaceStyle backFaceStyle)
        {
            FrontFaceStyle = frontFaceStyle;
            BackFaceStyle = backFaceStyle;
        }

        public SurfaceStyling(IEnumerable<SurfaceStyle> styles)
        {
            foreach (var style in styles)
            {
                switch (style.Side)
                {
                    case IfcSurfaceSide.POSITIVE:
                        FrontFaceStyle = style;
                        break;
                    case IfcSurfaceSide.NEGATIVE:
                        BackFaceStyle = style;
                        break;
                    case IfcSurfaceSide.BOTH:
                        FrontFaceStyle = style;
                        BackFaceStyle = style;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private SurfaceStyling()
        {
        }
        /// <summary>
        /// Sets the front and back material to the style
        /// </summary>
        /// <param name="frontFaceStyle"></param>
        public SurfaceStyling(SurfaceStyle frontFaceStyle)
        {
            FrontFaceStyle = frontFaceStyle;
            BackFaceStyle = frontFaceStyle;
        }


        public bool IsEmpty
        {
            get { return FrontFaceStyle == null && BackFaceStyle == null; }
        }

        /// <summary>
        /// Returns the front face if defined or the default
        /// </summary>
        /// <returns></returns>
        public SurfaceStyle FrontOrDefault(SurfaceStyle defaultStyle = null)
        {
            return FrontFaceStyle ?? defaultStyle;
        }

        /// <summary>
        /// Returns the front face if not nul or the back face if not null or the default
        /// </summary>
        /// <param name="defaultStyle"></param>
        /// <returns></returns>
        public SurfaceStyle FrontBackOrDefault(SurfaceStyle defaultStyle = null)
        {
            return FrontFaceStyle ?? BackFaceStyle ?? defaultStyle;
        }

        /// <summary>
        /// returns the back face if defined or the default
        /// </summary>
        /// <param name="defaultStyle"></param>
        /// <returns></returns>
        public SurfaceStyle BackOrDefault(SurfaceStyle defaultStyle = null)
        {
            return BackFaceStyle ?? defaultStyle;
        }
    }
}
