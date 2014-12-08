#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConnectionGeometry.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricConstraintResource
{
    /// <summary>
    ///   The IfcConnectionGeometry is used to describe the geometric and topological constraints that facilitate the physical connection of two objects.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcConnectionGeometry is used to describe the geometric and topological constraints that facilitate the physical connection of two objects. It is envisioned as a control that applies to the element connection relationships.
    ///   NOTE  The element connection relationship normally provides for a logical connection information, by referencing the relating and related elements. If in addition an IfcConnectionGeometry is provided, physical connection information is given by specifying exactly where at the relating and related element the element connection occurs. Using the eccentricity subtypes, the connection can also be described when there is a physical distance (or eccentricity) between the connection elements. 
    ///   The IfcConnectionGeometry allows for the provision of connection constraints between geometric and topological elements, the following connection geometry/topology types are in scope:
    ///   point | vertex point, 
    ///   curve | edge curve, 
    ///   surface | face surface, 
    ///   profile - NOTE  the profile (or port) connection type is deprecated since Release IFC2x Edition 2. 
    ///   HISTORY  New entity in IFC Release 1.5. 
    ///   IFC2x Edition 3 CHANGE  The definition of the subtypes has been enhanced by allowing either geometric representation items (point | curve | surface) or topological representation items with associated geometry (vertex point | edge curve | face  surface).
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcConnectionGeometry : ISupportChangeNotification, INotifyPropertyChanged, IPersistIfcEntity,
                                                  INotifyPropertyChanging
    {
        public override bool Equals(object obj)
        {
            // Check for null
            if (obj == null) return false;

            // Check for type
            if (this.GetType() != obj.GetType()) return false;

            // Cast as IfcRoot
            IfcConnectionGeometry root = (IfcConnectionGeometry)obj;
            return this == root;
        }
        public override int GetHashCode()
        {
            return _entityLabel.GetHashCode(); //good enough as most entities will be in collections of  only one model, equals distinguishes for model
        }

        public static bool operator ==(IfcConnectionGeometry left, IfcConnectionGeometry right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
                return true;

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
                return false;

            return (left.EntityLabel == right.EntityLabel) && (left.ModelOf == right.ModelOf);

        }

        public static bool operator !=(IfcConnectionGeometry left, IfcConnectionGeometry right)
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


        public abstract void IfcParse(int propIndex, IPropertyValue value);

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
            private event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
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

        #region ISupportChangeNotification Members

        void ISupportChangeNotification.NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public abstract string WhereRule();

        #endregion
    }
}