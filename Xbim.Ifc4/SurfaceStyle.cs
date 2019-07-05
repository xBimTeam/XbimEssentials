using System;
using System.ComponentModel;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PresentationAppearanceResource;

namespace Xbim.Ifc4
{
   
    public class SurfaceStyle :  IPhongMaterial
    {
        public static SurfaceStyle NullSurfaceStyle { get; private set; }

        static SurfaceStyle()
        {
            NullSurfaceStyle = new SurfaceStyle();
            
        }


      
        private readonly IIfcSurfaceStyle _surfaceStyle;
        public string Name { get; set; }

        public IfcSurfaceSide Side
        {
            get
            {
                if (_surfaceStyle != null)
                    return _surfaceStyle.Side;
                return IfcSurfaceSide.BOTH;
            }
        }

        string IPhongMaterial.Name { get; set; }
        public RgbaColour AmbientColour { get;  set; }
        public RgbaColour DiffuseColour { get;  set; }
        public RgbaColour EmissiveColour { get;  set; }
        public RgbaColour SpecularColour { get;  set; }
        public double SpecularShininess { get;  set; }

       
        public IIfcPixelTexture DiffuseMap;
       
        public IIfcPixelTexture DisplacementMap;
       
        public IIfcPixelTexture NormalMap;

        public SurfaceStyle(IIfcSurfaceStyle surfaceStyle)
        {
           
            _surfaceStyle = surfaceStyle;
            if (_surfaceStyle == null) Name = "NullStyle";
            else Name = _surfaceStyle.Name ?? "Default";
            InitialiseStyles();        
        }

        public SurfaceStyle()
        {
           
        }

        public SurfaceStyle(IPhongMaterial surfaceStyle)
        {
            AmbientColour = surfaceStyle.AmbientColour;
            DiffuseColour = surfaceStyle.DiffuseColour;
            DisplacementMap = surfaceStyle.DisplacementMap;
            EmissiveColour = surfaceStyle.EmissiveColour;
            Name = surfaceStyle.Name;
            NormalMap = surfaceStyle.NormalMap;
            SpecularColour = surfaceStyle.SpecularColour;
            SpecularShininess = surfaceStyle.SpecularShininess;
            DiffuseMap = surfaceStyle.DiffuseMap;
        }

        private void SurfaceStyle_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InitialiseStyles();
        }

        private void InitialiseStyles()
        {
            //set sensible defaults
            AmbientColour = new RgbaColour(0, 0, 0, 1);
            DiffuseColour = new RgbaColour(0, 0, 0, 1);
            SpecularColour = new RgbaColour(0, 0, 0, 1);
            EmissiveColour = new RgbaColour(0, 0, 0, 1);
            SpecularShininess = 0f;
            if (_surfaceStyle != null)
            {
                ((INotifyPropertyChanged) _surfaceStyle).PropertyChanged += SurfaceStyle_PropertyChanged;
                foreach (var surfaceStyle in _surfaceStyle.Styles)
                {
                    var shading = surfaceStyle as IIfcSurfaceStyleShading;
                    var rendering = surfaceStyle as IIfcSurfaceStyleRendering;
                    if (rendering != null)
                    {
                        AmbientColour = new RgbaColour(rendering.SurfaceColour, rendering.Transparency);

                        if (rendering.DiffuseColour != null)
                        {
                            var diffuseColour = rendering.DiffuseColour as IIfcColourRgb;
                            if (diffuseColour != null)
                                DiffuseColour = new RgbaColour(diffuseColour, rendering.Transparency);
                            else
                                DiffuseColour = AmbientColour*(IfcNormalisedRatioMeasure) rendering.DiffuseColour;
                        }
                        else
                        {
                            DiffuseColour = AmbientColour;
                        }
                        if (rendering.SpecularColour != null)
                        {
                            var specularColour = rendering.SpecularColour as IIfcColourRgb;
                            if (specularColour != null)
                                SpecularColour = new RgbaColour(specularColour, rendering.Transparency);
                            else
                                SpecularColour = AmbientColour*(IfcNormalisedRatioMeasure) rendering.SpecularColour;
                        }
                        if (rendering.SpecularHighlight is IfcSpecularExponent)
                            SpecularShininess = (IfcSpecularExponent) rendering.SpecularHighlight;
                        else
                            SpecularShininess = 0;
                    }
                    else if (shading != null)
                    {
                        AmbientColour = new RgbaColour(shading.SurfaceColour, shading.Transparency);
                    }
                    else if (surfaceStyle is IIfcSurfaceStyleLighting)
                    {
                        //TODO implement this function

                    }
                    else if (surfaceStyle is IIfcSurfaceStyleWithTextures)
                    {
                        //TODO implement this function
                    }
                    else if (surfaceStyle is IIfcExternallyDefinedSurfaceStyle)
                    {
                        //TODO implement this function
                    }
                    else if (surfaceStyle is IIfcSurfaceStyleRefraction)
                    {
                        //TODO implement this function
                    }
                }
            }
        }


        public SurfaceStyle Clone()
        {
            return new SurfaceStyle
            {
                AmbientColour = AmbientColour,
                DiffuseColour = DiffuseColour,
                DisplacementMap = DisplacementMap,
                EmissiveColour = EmissiveColour,
                Name = Name,
                NormalMap = NormalMap,              
                SpecularColour = SpecularColour,
                SpecularShininess = SpecularShininess,
                DiffuseMap = DiffuseMap,
            };
        }
        /// <summary>
        /// A style with a null diffuse colour is effectively invisible so we use this as empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return  Math.Abs(DiffuseColour.Alpha) < 1e-9 && Math.Abs(DiffuseColour.Red) < 1e-9 && Math.Abs(DiffuseColour.Green) < 1e-9 &&
                       Math.Abs(DiffuseColour.Blue) < 1e-9;
            }
        }

        IIfcPixelTexture IPhongMaterial.DiffuseMap
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        IIfcPixelTexture IPhongMaterial.DisplacementMap
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        IIfcPixelTexture IPhongMaterial.NormalMap
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }

    
}

