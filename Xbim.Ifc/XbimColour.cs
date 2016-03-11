﻿using System;
using System.Linq;
using Xbim.Ifc4.Interfaces;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Xbim.Ifc
{

    /// <summary>
    /// Represents a Colour in the model
    /// </summary>
    
    public class XbimColour
    {     
        /// <summary>
        /// Gets or sets Colour Name, defaults to its parts
        /// </summary>
        
        public String Name
        {
            get { if (string.IsNullOrWhiteSpace(_name)) return ""; else  return _name; }
            set { _name = value; }
        }

        public override bool Equals(object obj)
        {
           
            XbimColour col = obj as XbimColour;
            float tolerance = (float) 1e-5;
            if (col == null) return false;
            return Math.Abs(col.Red - Red) < tolerance &&
                    Math.Abs(col.Green - Green) < tolerance &&
                    Math.Abs(col.Blue - Blue) < tolerance &&
                    Math.Abs(col.Alpha - Alpha) < tolerance &&
                    Math.Abs(col.DiffuseFactor - DiffuseFactor) < tolerance &&
                    Math.Abs(col.TransmissionFactor - TransmissionFactor) < tolerance &&
                    Math.Abs(col.DiffuseTransmissionFactor - DiffuseTransmissionFactor) < tolerance &&
                    Math.Abs(col.ReflectionFactor - ReflectionFactor) < tolerance &&
                    Math.Abs(col.SpecularFactor - SpecularFactor) < tolerance;    
        }

        public override int GetHashCode()
        {
           
            return Red.GetHashCode() ^ Green.GetHashCode() ^ Blue.GetHashCode() ^ Alpha.GetHashCode() ^ DiffuseFactor.GetHashCode() ^
                TransmissionFactor.GetHashCode() ^ DiffuseTransmissionFactor.GetHashCode() ^ ReflectionFactor.GetHashCode() ^ SpecularFactor.GetHashCode();
        }

        /// <summary>
        /// True if the cuolour is not opaque
        /// </summary>
       
        public bool IsTransparent
        {
            get
            {
                return Alpha < 1;
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public XbimColour()
        {
           
        }

        /// <summary>
        /// Constructor for Material
        /// </summary>
        /// <param name="name">Material Name</param>
        /// <param name="red">Red component Value (range 0 to 1.0 inclusive)</param>
        /// <param name="green">Green Value (range 0 to 1.0 inclusive)</param>
        /// <param name="blue">Blue Value (range 0 to 1.0 inclusive)</param>
        /// <param name="alpha">Alpha Value (range 0 to 1.0 inclusive)</param>
        public XbimColour(string name, float red, float green, float blue, float alpha = 1.0f)
        {
            Name = name;
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Material Name</param>
        /// <param name="red">Red Value in range 0.0 to 1.0</param>
        /// <param name="green">Green Value in range 0.0 to 1.0</param>
        /// <param name="blue">Blue Value in range 0.0 to 1.0</param>
        /// <param name="alpha">Alpha Value in range 0.0 to 1.0</param>
        public XbimColour(string name, double red, double green, double blue, double alpha = 1.0)
            : this(name, (float)red, (float)green, (float)blue, (float)alpha)
        {
        }

        /// <summary>
        /// Creates a colour from Hue, Saturation and Value
        /// </summary>
        /// <param name="name">Color name</param>
        /// <param name="hue">range 0..360</param>
        /// <param name="saturation">range 0..1</param>
        /// <param name="value">range 0..1</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static XbimColour FromHSV(string name, double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            // value = value * 255;
            var v = Convert.ToDouble(value);
            var p = Convert.ToDouble(value * (1 - saturation));
            var q = Convert.ToDouble(value * (1 - f * saturation));
            var t = Convert.ToDouble(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return new XbimColour(name, v, t, p);
            else if (hi == 1)
                return new XbimColour(name, q, v, p);
            else if (hi == 2)
                return new XbimColour(name, p, v, t);
            else if (hi == 3)
                return new XbimColour(name, p, q, v);
            else if (hi == 4)
                return new XbimColour(name, t, p, v);
            else
                return new XbimColour(name, v, p, q);
        }

        /// <summary>
        /// Gets or sets Red component Value in range from 0.0 to 1.0
        /// </summary>
       
        public float Red
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Green component Value in range from 0.0 to 1.0
        /// </summary>
       
        public float Green
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Blue component Value in range from 0.0 to 1.0
        /// </summary>
       
        public float Blue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets transparency component Value in range from 0.0 to 1.0.
        /// A value of 0.0 is completely transparent.
        /// A value of 1.0 makes the colour fully opaque
        /// </summary>
        
        public float Alpha
        {
            get;
            set;
        }

        private String _name;
        
        public readonly float DiffuseFactor;
        
        public readonly float TransmissionFactor;
       
        public float DiffuseTransmissionFactor;
        
        public readonly float ReflectionFactor;
       
        public readonly float SpecularFactor;

        

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("R:{0} G:{1} B:{2} A:{3} DF:{4} TF:{5} DTF:{6} RF:{7} SF:{8}", Red, Green, Blue, Alpha,
                DiffuseFactor, TransmissionFactor, DiffuseTransmissionFactor, ReflectionFactor, SpecularFactor);
        }

        public static readonly XbimColour DefaultColour;


        static XbimColour()
        {
            DefaultColour = new XbimColour("Default", 1, 1, 1);
        }

        public XbimColour(IIfcSurfaceStyle style)
        {
            var rendering = style.Styles.OfType<IIfcSurfaceStyleShading>().FirstOrDefault();
            if (rendering != null)
            {
                var rgb = rendering.SurfaceColour;
                Red = (float)rgb.Red;
                Green = (float)rgb.Green;
                Blue = (float)rgb.Blue;
                Alpha = (float)(1.0 - rendering.Transparency ?? 1.0);
            }
        }

        internal XbimColour(IIfcColourRgb rgbColour)
        {
            Red = (float)rgbColour.Red;
            Green = (float)rgbColour.Green;
            Blue = (float)rgbColour.Blue;
            Alpha = 1;
           // this.Name = string.Format("{0},A={1},D={2},S={3},T={4},R={5},RGB={6}", rgbColour.Name, Alpha, DiffuseFactor, SpecularFactor, TransmissionFactor, ReflectionFactor, rgbColour.EntityLabel);
            
        }

        public XbimColour(IIfcColourRgb ifcColourRgb, double opacity = 1.0, double diffuseFactor = 1.0 , double specularFactor = 0.0, double transmissionFactor = 1.0, double reflectanceFactor = 0.0)
            :this(ifcColourRgb)
        {
            Alpha = (float)opacity;
            DiffuseFactor = (float)diffuseFactor;
            SpecularFactor = (float)specularFactor;
            TransmissionFactor = (float)transmissionFactor;
            ReflectionFactor = (float)reflectanceFactor;
        }

        
    }
}

