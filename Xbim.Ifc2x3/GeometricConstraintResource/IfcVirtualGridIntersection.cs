#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    VirtualGridIntersection.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using System.ComponentModel;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricConstraintResource
{
    /// <summary>
    ///   The IfcVirtualGridIntersection defines the derived location of the intersection between two grid axes. Offset values may be given to set an offset distance to the grid axis for the calculation of the virtual grid intersection.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcVirtualGridIntersection defines the derived location of the intersection between two grid axes. Offset values may be given to set an offset distance to the grid axis for the calculation of the virtual grid intersection.
    ///   The two intersecting axes (IntersectingAxes) define the intersection point, which exact location (in terms of the Cartesian point representing the intersection) has to be calculated from the geometric representation of the two participating curves.
    ///   Offset values may be given (OffsetDistances). If given, the position within the list of OffsetDistances corresponds with the position within the list of IntersectingAxes. Therefore:
    ///   OffsetDistances[1] sets the offset to IntersectingAxes[1], 
    ///   OffsetDistances[2] sets the offset to IntersectingAxes[2], and 
    ///   OffsetDistances[3] sets the offset to the virtual intersection in direction of the orientation of the cross product of IntersectingAxes[1] and the orthogonal complement of the IntersectingAxes[1] (which is the positive or negative direction of the z axis of the design grid position). 
    ///   HISTORY: New entity in IFC Release 1.5. The entity name was changed from IfcConstraintRelIntersection in IFC Release 2x.
    ///   Geometry use definitions:
    ///   The following figures explain the usage of the OffsetDistances and IntersectingAxes attributes.
    ///   Two offset distances given, the virtual intersection is defined in the xy plane of the grid axis placement. 
    ///   Three offset distances given, the virtual intersection is defined by an offset (in direction of the z-axis of the design grid placement) to the virtual intersection in the xy plane of the grid axis placement. 
    ///   The distance of the offset curve (OffsetDistances[n]) is measured from the basis curve. The distance may be positive, negative or zero. A positive value of distance defines an offset in the direction which is normal to the curve in the sense of an anti-clockwise rotation through 90 degrees from the tangent vector T at the given point. (This is in the direction of orthogonal complement(T).) This can be reverted by the SameSense attribute at IfcGridAxis which may switch the sense of the AxisCurve.
    ///   Illustration
    ///   example of a negative offset 
    ///   IntersectingAxes[1].AxisCurve is an IfcTrimmedCurve with an IfcCircle as BasisCurve and SenseAgreement = TRUE. 
    ///   IntersectingAxes[1].SameSense = TRUE. 
    ///   OffsetDistances[1] is a negative length measure 
    ///   the figure
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcVirtualGridIntersection : INotifyPropertyChanged, ISupportChangeNotification, IPersistIfcEntity,
                                    INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcVirtualGridIntersection root = (IfcVirtualGridIntersection)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcVirtualGridIntersection left, IfcVirtualGridIntersection right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcVirtualGridIntersection left, IfcVirtualGridIntersection right)
        {
            return !(left == right);
        }
        #region IPersistIfcEntity Members

        private int _entityLabel;
		bool _activated;

        private IModel _model;

        public IModel ModelOf
        {
            get { return _model; }
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _activated=activated;
			_model = model;
            _entityLabel = entityLabel;
        }

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        public int EntityLabel
        {
            get { return _entityLabel; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock(this) { if (_model != null && !_activated) _activated = _model.Activate(this, false)>0;  }
            if (write) _model.Activate(this, write);
        }

        #endregion

        XbimListUnique<IfcGridAxis> _intersectingAxes;

        XbimList<IfcLengthMeasure> _offsetDistances;

        public IfcVirtualGridIntersection()
        {
            _intersectingAxes = new XbimListUnique<IfcGridAxis>(this);
            _offsetDistances = new XbimList<IfcLengthMeasure>(this);
        }
        /// <summary>
        ///   Two grid axes which intersects at exactly one intersection (see also informal proposition at IfcGrid). If attribute OffsetDistances is omited, the intersection defines the placement or ref direction of a grid placement directly. If OffsetDistances are given, the intersection is defined by the offset curves to the grid axes.
        /// </summary>
        public XbimListUnique<IfcGridAxis> IntersectingAxes
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _intersectingAxes;
            }
            set
            {
                this.SetModelValue(this, ref _intersectingAxes, value, v => IntersectingAxes = v, "IntersectingAxes");
            }
            
        }

        /// <summary>
        ///   Offset distances to the grid axes. If given, it defines virtual offset curves to the grid axes. The intersection of the offset curves specify the virtual grid intersection.
        /// </summary>
        public XbimList<IfcLengthMeasure> OffsetDistances
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _offsetDistances;
            }
            set
            {
                this.SetModelValue(this, ref _offsetDistances, value, v => OffsetDistances = v, "OffsetDistances");
            }
           
        }
        #region ISupportIfcParser Members

        public void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    ((IXbimNoNotifyCollection)_intersectingAxes).Add((IfcGridAxis)value.EntityVal);
                    break;
                case 1:
                    ((IXbimNoNotifyCollection)_offsetDistances).Add(new IfcLengthMeasure(value.RealVal));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("P21 index value out of range in {0}",
                                                                        this.GetType().Name));
            }
        }


        public string WhereRule()
        {
            return "";
        }

        #endregion

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
        private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        #endregion

        #region ISupportChangeNotification Members

        void ISupportChangeNotification.NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void ISupportChangeNotification.NotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        [field: NonSerialized] //don't serialize events
        private event PropertyChangingEventHandler PropertyChanging;

        event PropertyChangingEventHandler INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }

        #endregion

    }
}