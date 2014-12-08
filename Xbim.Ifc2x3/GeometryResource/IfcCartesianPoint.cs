#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCartesianPoint.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.Geometry;


#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class CartesianPointList : XbimList<IfcCartesianPoint>
    {
        public CartesianPointList(IPersistIfcEntity owner)
            : base(owner, 5)
        {
        }

        internal CartesianPointList(IPersistIfcEntity owner, int capacity)
            : base(owner, capacity)
        {
        }


        public override string ToString()
        {
            if (Count == 0) return "";
            StringBuilder str = new StringBuilder(Count*32); //rough guess at string size
            str.AppendFormat("{0}D", this[0].Dim);
            foreach (IfcCartesianPoint pt in this)
            {
                str.AppendFormat(",{0}", pt);
            }
            return str.ToString();
        }
    }


    public interface ICoordinateList : IList<IfcLengthMeasure>, ExpressEnumerable
    {
        
    }


    /// <summary>
    ///   A point defined by its coordinates in a two or three dimensional rectangular Cartesian coordinate system, or in a two dimensional parameter space.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A point defined by its coordinates in a two or three dimensional rectangular Cartesian coordinate system, or in a two dimensional parameter space. The entity is defined in a two or three dimensional space. 
    ///   Definition from IAI: The derived attribute Dim has been added (see also note at IfcGeometricRepresentationItem). The WR1 was added to constrain the usage of IfcCartesianPoint in the context of IFC geometry. For the purpose of defining geometry in IFC only two and three dimensional Cartesian points are used. 
    ///   NOTE: Corresponding STEP entity : cartesian_point, please refer to ISO/IS 10303-42:1994, p. 23 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 1.0 
    ///   Formal Propositions:
    ///   WR1   :   Only two or three dimensional points shall be used for the purpose of defining geometry in this IFC Resource
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcCartesianPoint : IfcPoint, IfcTrimmingSelect, IVertex3D, ICoordinateList, IIfcCartesianPoint
    {
        #region Fields

        private double _x;
        private double _y;
        private double _z;
       
        #endregion

        private class CoordinateListEnumerator : IEnumerator<IfcLengthMeasure>
        {
            private int pos = -1;
            private readonly ICoordinateList _coordinate;
            private IfcLengthMeasure _current = double.NaN;

            public CoordinateListEnumerator(ICoordinateList coord)
            {
                _coordinate = coord;
            }

            #region IEnumerator<double> Members

            public IfcLengthMeasure Current
            {
                get { return _coordinate[pos]; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                pos++;
                if (pos > 2 || _coordinate.Count == 0) return false;
                _current = _coordinate[pos];
                return !double.IsNaN(_current);
            }

            public void Reset()
            {
                pos = -1;
            }

            #endregion
        }

        #region Constructors

        public IfcCartesianPoint()
        {
            _x = double.NaN;
            _y = double.NaN;
            _z = double.NaN;
        }


        public IfcCartesianPoint(XbimPoint3D pt3D)
            : this(pt3D.X, pt3D.Y, pt3D.Z)
        {
        }


        public IfcCartesianPoint(IfcCartesianPoint cp)
        {
            _x = cp.X;
            _y = cp.Y;
            _z = cp.Z;
        }

        public IfcCartesianPoint(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public IfcCartesianPoint(double x, double y)
        {
            _x = x;
            _y = y;
            _z = double.NaN;
        }



        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The first, second, and third coordinate of the point location. If placed in a two or three dimensional rectangular Cartesian coor
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.List, 1, 3)]
        public ICoordinateList Coordinates
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return this;
            }
            set
            {
                for (int i = 0; i < value.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            this.SetModelValue(this, ref _x, value[0], v => _x = v, "X");
                            break;
                        case 1:
                            this.SetModelValue(this, ref _y, value[1], v => _y = v, "Y");
                            break;
                        case 2:
                            this.SetModelValue(this, ref _z, value[2], v => _z = v, "Z");
                            break;
                    }
                }
            }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
            {
               
                ((ICoordinateList) this).Add(value.RealVal);
            }
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        /// <summary>
        ///   Derived. The space dimensionality of this class, determined by the number of coordinates in the List of Coordinates.
        /// </summary>
        
        public override IfcDimensionCount Dim
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                if (double.IsNaN(_x)) return 0;
                if (double.IsNaN(_y)) return 1;
                if (double.IsNaN(_z)) return 2;
                return 3;
            }
        }

        /// <summary>
        ///   If the cartesian is 3D then returns Point3D or throws exception
        /// </summary>
        public XbimPoint3D XbimPoint3D()
        {
            ((IPersistIfcEntity) this).Activate(false);
            return new XbimPoint3D(_x, _y, _z);
        }

        public double Distance(IfcCartesianPoint p)
        {
            return Math.Sqrt(DistanceSquared(p));
        }
        public double DistanceSquared(IfcCartesianPoint p)
        {
            double d = 0, dd;
            double x1, y1, z1, x2, y2, z2;
            XYZ(out x1, out y1, out z1);
            p.XYZ(out x2, out y2, out z2);
            dd = x1; dd -= x2; dd *= dd; d += dd;
            dd = y1; dd -= y2; dd *= dd; d += dd;
            dd = z1; dd -= z2; dd *= dd; d += dd;
            return d;
        }

        public bool IsEqual(IfcCartesianPoint p, double tolerance)
        {
            return DistanceSquared(p) <= (tolerance*tolerance); 
        }

        public double X
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _x;
            }
            set { this.SetModelValue(this, ref _x, value, v => _x = v, "X"); }
        }

        public void XYZ(out double x, out double y, out double z)
        {
            if (Dim == 3)
            {
               
                x = _x; y = _y; z = _z;
            }
            else if (Dim == 2)
            {
               
                x = _x; y = _y; z = 0;
            }
            else
            {
                z = y = x = double.NaN;
            }
        }

        public double Y
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _y;
            }
            set { this.SetModelValue(this, ref _y, value, v => _y = v, "Y"); }
        }

        public double Z
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _z;
            }
            set { this.SetModelValue(this, ref _z, value, v => _z = v, "Z"); }
        }

        public void SetXYZ(double x, double y, double z)
        {
            this.SetModelValue(this, ref _x, x, v => _x = v, "X");
            this.SetModelValue(this, ref _y, y, v => _y = v, "Y");
            this.SetModelValue(this, ref _z, z, v => _z = v, "Z");
        }

        public void SetXY(double x, double y)
        {
            this.SetModelValue(this, ref _x, x, v => _x = v, "X");
            this.SetModelValue(this, ref _y, y, v => _y = v, "Y");
            this.SetModelValue(this, ref _z, double.NaN, v => _z = v, "Z");
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", X, Y, Z);
        }


        public override string WhereRule()
        {
            IfcDimensionCount d = Dim;
            if (d < 2 || d > 3)
                return
                    "WR1 CartesianPoint : Only two or three dimensional points shall be used for the purpose of defining geometry in this IFC Resource.";
            else
                return "";
        }

        #region IEnumerable<LengthMeasure> Members

        public IEnumerator<IfcLengthMeasure> GetEnumerator()
        {
            ((IPersistIfcEntity) this).Activate(false);
            return new CoordinateListEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            ((IPersistIfcEntity) this).Activate(false);
            return new CoordinateListEnumerator(this);
        }

        #endregion

        #region ExpressEnumerable Members

        public string ListType
        {
            get { return "list"; }
        }

        #endregion

        #region IList<LengthMeasure> Members

        public int IndexOf(IfcLengthMeasure item)
        {
            ((IPersistIfcEntity) this).Activate(false);
            if (double.IsNaN(item)) throw new Exception("Cannot treat a NAN as a coordinate value");
            if (_x == item) return 0;
            if (_y == item) return 1;
            if (_z == item) return 2;
            return -1;
        }

        public void Insert(int index, IfcLengthMeasure item)
        {
            throw new Exception("Insert operations not supported on CartesianPoint Coordinates");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("RemoveAt operations not supported on CartesianPoint Coordinates");
        }

        public IfcLengthMeasure this[int index]
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                switch (index)
                {
                    case 0:
                        return _x;
                    case 1:
                        return _y;
                    case 2:
                        return _z;
                    default:
                        throw new Exception("Index out of bounds for CartesianPoint");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.SetModelValue(this, ref _x, value, v => _x = v, "X");
                        break;
                    case 1:
                        if (double.IsNaN(_x)) this.SetModelValue(this, ref _x, 0, v => _x = v, "X");
                        this.SetModelValue(this, ref _y, value, v => _y = v, "Y");
                        break;
                    case 2:
                        if (double.IsNaN(_x)) this.SetModelValue(this, ref _x, 0, v => _x = v, "X");
                        if (double.IsNaN(_y)) this.SetModelValue(this, ref _y, 0, v => _y = v, "Y");
                        this.SetModelValue(this, ref _z, value, v => _z = v, "Z");
                        break;
                    default:
                        throw new Exception("Index out of bounds for CartesianPoint");
                }
            }
        }

        #endregion

        #region ICollection<LengthMeasure> Members

        public void Add(IfcLengthMeasure item)
        {
            if (double.IsNaN(_x)) _x = item;
            else if (double.IsNaN(_y)) _y = item;
            else if (double.IsNaN(_z)) _z = item;
            else throw new Exception("Index out of bounds for CartesianPoint in Add");
        }

        public void Clear()
        {
            _x = double.NaN;
            _y = double.NaN;
            _z = double.NaN;
        }

        public bool Contains(IfcLengthMeasure item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IfcLengthMeasure[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                if (double.IsNaN(_x)) return 0;
                if (double.IsNaN(_y)) return 1;
                if (double.IsNaN(_z)) return 2;
                return 3;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IfcLengthMeasure item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ExpressEnumerable Members

        public void Add(object o)
        {
            if (double.IsNaN(_x)) _x = (IfcLengthMeasure) o;
            else if (double.IsNaN(_y)) _y = (IfcLengthMeasure) o;
            else if (double.IsNaN(_z)) _z = (IfcLengthMeasure) o;
            else throw new Exception("Index out of bounds for CartesianPoint in Add");
        }

        #endregion

        #region ExpressEnumerable Members

        public IPersistIfcEntity OwningEntity
        {
            get { return this; }
            set
            {
                // throw new NotImplementedException("You can set an owning entity for CartesianPoint");
            }
        }

        #endregion
    }

    //#region CartesianPoint Converter

    //public class CartesianPointConverter : System.ComponentModel.TypeConverter
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

    //        if (destinationType == typeof(string))
    //        {
    //            CartesianPoint pt = value as CartesianPoint;
    //            if (pt != null)
    //                switch (pt.Dim)
    //                {
    //                    case 1:
    //                        return String.Format("{0}", pt.X);
    //                    case 2:
    //                        return String.Format("{0},{1}", pt.X, pt.Y);
    //                    case 3:
    //                        return String.Format("{0},{1},{2}", pt.X, pt.Y, pt.Z);
    //                    default:
    //                        break;
    //                }
    //        }
    //        return base.ConvertTo(context, culture, value, destinationType);
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
    //            ICoordinateList coords = new ICoordinateList(DoubleCollection.Parse(str).Cast<LengthMeasure>()) ;
    //            return new CartesianPoint(coords);
    //        }
    //        else
    //            return base.ConvertFrom(context, culture, value);
    //    }
    //}
    //#endregion
}
