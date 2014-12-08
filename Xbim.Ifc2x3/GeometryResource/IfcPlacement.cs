#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPlacement.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;

using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A placement entity defines the local environment for the definition of a geometry item.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A placement entity defines the local environment for the definition of a geometry item. It locates the item to be defined and, in the case of the axis placement subtypes, gives its orientation. 
    ///   Additional definition from ISO/WD SC4/WG12/N071 Part42.2 geometry_schema: A placement locates a geometric item with respect to the coordinate system of its geometric context. 
    ///   Definition from IAI: The IfcPlacement is an abstract supertype not to be directly instantiated, whereas the STEP P42 entity placement can be instantiated to define a placement without orientation. The derived attribute Dim has been added, see also note at IfcGeometricRepresentationItem. 
    ///   NOTE: Corresponding STEP entity: placement. Please refer to ISO/IS 10303-42:1994, p. 27 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 1.0
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcPlacement : IfcGeometricRepresentationItem
    {
        #region Fields

        private IfcCartesianPoint _location;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The geometric position of a reference point, such as the center of a circle, of the item to be located.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcCartesianPoint Location
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _location;
            }
            set { this.SetModelValue(this, ref _location, value, v => Location = value, "Location"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
            {
                _location = (IfcCartesianPoint) value.EntityVal;
            }
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        /// <summary>
        ///   Derived. The space dimensionality of this class, derived from the dimensionality of the location.
        /// </summary>
        
        public IfcDimensionCount Dim
        {
            get { return Location == null ? (short) 0 : Location.Dim; }
        }

        public override string ToString()
        {
            return string.Format("L={0}", Location);
        }
    }

    //#region Converter

    //public class PlacementConverter : System.ComponentModel.TypeConverter
    //{
    //    public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
    //    {
    //        if (destinationType == typeof(string))
    //            return true;
    //        else
    //            return base.CanConvertTo(context, destinationType);
    //    }

    //    public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    //    {
    //        IfcPlacement ax = value as IfcPlacement;
    //        if (ax != null && destinationType == typeof(string))
    //        {
    //            return ax.ToString();
    //        }
    //        else
    //            return base.ConvertTo(context, culture, value, destinationType);
    //    }

    //    public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
    //    {

    //        if (sourceType == typeof(string))
    //            return true;
    //        else
    //            return base.CanConvertFrom(context, sourceType);
    //    }

    //    public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    //    {
    //        string str = value as string;
    //        if (str != null)
    //        {
    //            str = str.ToUpper();
    //            TrimmingSelectList lst = new TrimmingSelectList();

    //            string[] tokens = str.Split(new char[] {'=', ';'}, StringSplitOptions.RemoveEmptyEntries);
    //            int max = tokens.Length;
    //            IfcDirection dir = null;
    //            IfcDirection axis = null;
    //            IfcCartesianPoint loc = null;
    //            for (int i = 0; i < max; i+=2)
    //            {
    //                int dblIdx = i + 1;
    //                if (dblIdx < max)
    //                {
    //                    DoubleCollection dc = DoubleCollection.Parse(tokens[dblIdx]);
    //                    if (tokens[i].Contains("L"))
    //                    {
    //                        if (dc.Count == 2) loc = new IfcCartesianPoint(dc[0], dc[1]);
    //                        else if (dc.Count == 3) loc = new IfcCartesianPoint(dc[0], dc[1], dc[2]);

    //                    }
    //                    else if (tokens[i].Contains("D"))
    //                    {
    //                        if (dc.Count == 2) dir = new IfcDirection(dc[0], dc[1]);
    //                        else if (dc.Count == 3) dir = new IfcDirection(dc[0], dc[1], dc[2]);
    //                    }
    //                    else if (tokens[i].Contains("A"))
    //                    {
    //                        if (dc.Count == 2) axis = new IfcDirection(dc[0], dc[1]);
    //                        else if (dc.Count == 3) axis = new IfcDirection(dc[0], dc[1], dc[2]);
    //                    }
    //                }
    //            }
    //            if (loc !=null && loc.Dim == 2)
    //                return new IfcAxis2Placement2D() { Location = loc, RefDirection = dir };
    //            else if (loc != null && loc.Dim == 3)
    //                return new IfcAxis2Placement3D() { Location = loc, RefDirection = dir, Axis = axis };
    //            else
    //                return base.ConvertFrom(context, culture, value);

    //        }
    //        else
    //            return base.ConvertFrom(context, culture, value);
    //    }
    //}
    //#endregion
}