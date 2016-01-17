using Xbim.Ifc4.Interfaces;
using Xbim.IO.Parser;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Xbim.Ifc
{
    /// <summary>
    /// Class to hold the surface style or texture of an object, corresponds to IIfcSurfaceStyle and OpenGL Texture
    /// Does not handle bitmap textured surfaces etc at present
    /// </summary>
   
    public class XbimTexture 
    {
       
        public readonly XbimColourMap ColourMap = new XbimColourMap();
        /// <summary>
        /// The object that this style defines
        /// </summary>
        
        public int DefinedObjectId{get;set;}

        bool _renderBothFaces = true;
        bool _switchFrontAndRearFaces;
        
        public XbimColour DiffuseTransmissionColour;
        
        public XbimColour TransmissionColour;
       
        public XbimColour DiffuseReflectionColour;
       
        public XbimColour ReflectanceColour;
        public static XbimTexture Create(IIfcSurfaceStyle surfaceStyle)
        {
            var texture = new XbimTexture();
            return texture.CreateTexture(surfaceStyle);
        }
        private XbimTexture CreateTexture(IIfcSurfaceStyle surfaceStyle)
        {
            DefinedObjectId = surfaceStyle.EntityLabel;
            //set render one or both faces
            _renderBothFaces = (surfaceStyle.Side == IfcSurfaceSide.BOTH);
            //switch if required
            _switchFrontAndRearFaces = (surfaceStyle.Side == IfcSurfaceSide.NEGATIVE);
            ColourMap.Clear();
            foreach (var style in surfaceStyle.Styles)
            {
                if (style is IIfcSurfaceStyleRendering) AddColour((IIfcSurfaceStyleRendering)style);
                else if (style is IIfcSurfaceStyleShading) AddColour((IIfcSurfaceStyleShading)style);
                else if (style is IIfcSurfaceStyleLighting) AddLighting((IIfcSurfaceStyleLighting)style);
            }
            return this;
        }

        public override int GetHashCode()
        {

            int hash = ColourMap.GetHashCode() ^ (_renderBothFaces ? 1 : 0) ^ (_switchFrontAndRearFaces ? 1 : 0);
            if(DiffuseTransmissionColour!=null) hash^=DiffuseTransmissionColour.GetHashCode();
            if(TransmissionColour!=null) hash^=TransmissionColour.GetHashCode();
            if (DiffuseReflectionColour != null) hash ^= DiffuseReflectionColour.GetHashCode();
            if (ReflectanceColour != null) hash ^= ReflectanceColour.GetHashCode();
            return hash;    
        }

        public override bool Equals(object obj)
        {
           
            XbimTexture t = obj as XbimTexture;
            if (t == null) return false;
            bool isSame = t.ColourMap.Equals(ColourMap) && t.RenderBothFaces == RenderBothFaces && t.SwitchFrontAndRearFaces == SwitchFrontAndRearFaces &&
                         t.DiffuseTransmissionColour == DiffuseTransmissionColour && t.TransmissionColour == TransmissionColour &&
                         t.DiffuseReflectionColour == DiffuseReflectionColour && t.ReflectanceColour == ReflectanceColour;
            return isSame;
        }

        private void AddColour(IIfcSurfaceStyleShading shading)
        {
            ColourMap.Add(new XbimColour(shading.SurfaceColour));
        }

        private void AddColour(IIfcSurfaceStyleRendering rendering)
        {
            var alpha = 1.0;
            if (rendering.Transparency.HasValue) alpha =  1.0 - (double)(rendering.Transparency);
            if (rendering.DiffuseColour is Ifc4.MeasureResource.IfcNormalisedRatioMeasure)
            {
                ColourMap.Add(new XbimColour(
                    rendering.SurfaceColour,
                    alpha,
                    (Ifc4.MeasureResource.IfcNormalisedRatioMeasure)rendering.DiffuseColour
                    ));

            }
            else if (rendering.DiffuseColour is IIfcColourRgb)
            {
                ColourMap.Add(new XbimColour(
                    (IIfcColourRgb)rendering.DiffuseColour,
                    alpha
                    ));

            }
            else if (rendering.DiffuseColour == null)
            {
                ColourMap.Add(new XbimColour(
                    rendering.SurfaceColour,
                    alpha
                    ));
            }
            else if (rendering.SpecularColour is Ifc4.MeasureResource.IfcNormalisedRatioMeasure) //getting key duplication on some ifc models so add else if
            {
                ColourMap.Add(new XbimColour(
                    rendering.SurfaceColour,
                   alpha,
                    (Ifc4.MeasureResource.IfcNormalisedRatioMeasure)(rendering.SpecularColour)
                    ));
            }
            else if (rendering.SpecularColour is IIfcColourRgb)
            {
                ColourMap.Add(new XbimColour(
                    (IIfcColourRgb)rendering.SpecularColour,
                    alpha
                    ));

            }
        }

        private void AddLighting(IIfcSurfaceStyleLighting lighting)
        {
            DiffuseReflectionColour = new XbimColour(lighting.DiffuseReflectionColour);
            DiffuseTransmissionColour = new XbimColour(lighting.DiffuseTransmissionColour);
            TransmissionColour = new XbimColour(lighting.TransmissionColour);
            ReflectanceColour = new XbimColour(lighting.ReflectanceColour);
        }

        public static XbimTexture Create(IIfcColourRgb colour)
        {
            var texture = new XbimTexture();
            return texture.CreateTexture(colour);
        }

        private XbimTexture CreateTexture(IIfcColourRgb colour)
        {
            DefinedObjectId = colour.EntityLabel;
            ColourMap.Clear();
            ColourMap.Add(new XbimColour(colour));
            return this;
        }

        public static XbimTexture Create(IIfcSurfaceStyleRendering rendering)
        {
            var texture = new XbimTexture();
            return texture.CreateTexture(rendering);
        }
        private XbimTexture CreateTexture(IIfcSurfaceStyleRendering rendering)
        {
            DefinedObjectId = rendering.EntityLabel; 
            ColourMap.Clear();
            AddColour(rendering);
            return this;
            
        }
        public static XbimTexture Create(IIfcSurfaceStyleShading shading)
        {
            var texture = new XbimTexture();
            return texture.CreateTexture(shading);
        }
        private XbimTexture CreateTexture(IIfcSurfaceStyleShading shading)
        {
            DefinedObjectId = shading.EntityLabel; 
            ColourMap.Clear();
            if (shading is IIfcSurfaceStyleRendering)
                AddColour((IIfcSurfaceStyleRendering)shading);
            else
                AddColour(shading);
            return this;
        }
        public static XbimTexture Create(byte red = 255, byte green = 255, byte blue = 255, byte alpha = 255)
        {
            var texture = new XbimTexture();
            return texture.CreateTexture(red, green, blue, alpha);
        }

        /// <summary>
        /// Sets the texture property for a single colour
        /// </summary>
        /// <param name="red">the red component of the colour in a range 0 to 255</param>
        /// <param name="green">the green component of the colour in a range 0 to 255</param>
        /// <param name="blue">the blue component of the colour in a range 0 to 255</param>
        /// <param name="alpha">opaqueness of the colour in a range 0 to 255 (255 meaning completely opaque)</param>
        /// <returns></returns>
        private XbimTexture CreateTexture(byte red = 255, byte green = 255, byte blue = 255, byte alpha = 255)
        {

            ColourMap.Clear();
            ColourMap.Add(new XbimColour("C1",
                (float)red / 255,
                (float)green / 255,
                (float)blue / 255,
                (float)alpha / 255
                ));
            return this;
        }

        /// <summary>
        /// Sets the texture property for a single colour
        /// </summary>
        /// <param name="red">the red component of the colour in a range 0.0f to 1.0f</param>
        /// <param name="green">the green component of the colour in a range 0.0f to 1.0f</param>
        /// <param name="blue">the blue component of the colour in a range 0.0f to 1.0f</param>
        /// <param name="alpha">opaqueness of the colour in a range 0.0f to 1.0f (1.0f meaning completely opaque)</param>
        /// <returns></returns>
        public XbimTexture CreateTexture(float red = 1.0f, float green = 1.0f, float blue = 1.0f, float alpha = 1.0f)
        {
            ColourMap.Clear();
            ColourMap.Add(new XbimColour("C1", red, green, blue, alpha));
            return this;
        }

       
        public bool IsTransparent
        {
            get { return ColourMap.IsTransparent; }
        }
       
        public bool RenderBothFaces
        {
            get { return _renderBothFaces; }
        }
      
        public bool SwitchFrontAndRearFaces
        {
            get { return _switchFrontAndRearFaces; }
        }

        private XbimTexture CreateTexture(XbimColour colour)
        {
            ColourMap.Clear();
            AddColour(colour);
            return this;
        }
        public static XbimTexture Create(XbimColour colour)
        {
            var texture = new XbimTexture();
            return texture.CreateTexture(colour);
        }
        private void AddColour(XbimColour colour)
        {
            if (string.IsNullOrEmpty(colour.Name))
                colour.Name = "C" + (ColourMap.Count + 1);
            ColourMap.Add(colour);
        }
    }
}
