#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcVector.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using System.Globalization;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class IfcVector : IfcGeometricRepresentationItem, IfcVectorOrDirection
    {
        #region Fields

        private IfcLengthMeasure _magnitude;
        private IfcDirection _orientation;

        #endregion

        #region Constructors

        public IfcVector()
        {
        }


        public IfcVector(IfcDirection dir, IfcLengthMeasure magnitude)
        {
            _orientation = dir;
            _magnitude = magnitude;
        }

        public IfcVector(double x, double y, double z, IfcLengthMeasure magnitude)
        {
            _orientation = new IfcDirection(x, y, z);
            _magnitude = magnitude;
        }

        public IfcVector(double x, double y, IfcLengthMeasure magnitude)
        {
            _orientation = new IfcDirection(x, y);
            _magnitude = magnitude;
        }

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The direction of the vector.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcDirection Orientation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _orientation;
            }
            set { this.SetModelValue(this, ref _orientation, value, v => Orientation = v, "Orientation"); }
        }

        /// <summary>
        ///   The magnitude of the vector. All vectors of Magnitude 0.0 are regarded as equal in value regardless of the orientation attribute.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcLengthMeasure Magnitude
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _magnitude;
            }
            set { this.SetModelValue(this, ref _magnitude, value, v => Magnitude = v, "Magnitude"); }
        }

        /// <summary>
        ///   Derived. The space dimensionality of this class, it is derived from Orientation
        /// </summary>
        public IfcDimensionCount Dim
        {
            get { return Orientation.Dim; }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _orientation = (IfcDirection) value.EntityVal;
                    break;
                case 1:
                    _magnitude = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion


        /// <summary>
        ///   Converts an Ifc 3D vector to an Xbim Vector3D
        /// </summary>
        /// <returns></returns>
        public XbimVector3D XbimVector3D()
        {
            XbimVector3D vec;
            if (Orientation.Dim > 2)
                vec = new XbimVector3D(Orientation.X, Orientation.Y, Orientation.Z);
            else if (Orientation.Dim == 2)
                vec = new XbimVector3D(Orientation.X, Orientation.Y, 0);
            else
                vec = new XbimVector3D();
            vec.Normalize(); //orientation is not normalized
            vec *= Magnitude;
            return vec;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Magnitude, Orientation);
        }

        
        public override string WhereRule()
        {
            if (Magnitude < 0)
                return "WR1 Vector : The magnitude shall be positive or zero.\n";
            else
                return "";
        }


        public static IfcVector CrossProduct(IfcVector v1, IfcVector v2)
        {
            if (v1.Dim == 3 && v2.Dim == 3)
            {
                XbimVector3D v3D = v1.XbimVector3D().CrossProduct(v2.XbimVector3D());
                return new IfcVector(v3D.X, v3D.Y, v3D.Z, v3D.Length);
            }
            else
            {
                throw new ArgumentException("CrossProduct: Both Vectors must have the same dimensionality");
            }
        }
    }

   
}
