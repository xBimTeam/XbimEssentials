using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.PresentationDefinitionResource
{
    /// <summary>
    /// An IfcVertexBasedTextureMap provides the mapping of the 2-dimensional texture coordinates (S, T) to the vertices of a single surface onto which it is mapped. For each vertex coordinates, provided by IfcCartesianPoin, a set of 2 (S, T) texture coordinates are given.
    /// 
    /// The IfcVertexBasedTextureMap provides two corresponding lists:
    /// 
    ///     a list of TexturePoints, given by min. of 3 IfcCartesianPoint's.
    ///     a list of TextureVertices, given by min. of 3 IfcTextureVertex's.
    /// 
    /// These corresponding lists are:
    /// 
    ///     TextureVertices -- LIST [3:?] --o IfcTextureVertex  -- LIST [2:2] --o IfcParameterValue
    ///     TexturePoints   -- LIST [3:?] --o IfcCartesianPoint -- LIST [3:3] --o IfcLengthMeasure
    /// 
    /// Each texture vertex (given as S, T coordinates of 2 dimension) corresponds to the geometric coordinates (given as X, Y, and Z coordinates of 3 dimensions) of the Cartesian point, All Cartesian points within the list of shall lie within a plane. 
    /// 
    /// Informal propositions:
    ///
    ///    The list of TextureVertices shall correspond to the list of TexturePoints.
    ///    All Cartesian points of the list of TexturePoints shall lie in one plane
    ///    The references points shall be part of the vertex based geometry to which the annotation surface with textures is assigned.
    ///
    /// </summary>
    [IfcPersistedEntity]
    public class IfcVertexBasedTextureMap : IPersistIfcEntity, ISupportChangeNotification
    {
        public IfcVertexBasedTextureMap()
        {
            _TexturePoints = new XbimSet<IfcCartesianPoint>(this);
            _TextureVertices = new XbimSet<IfcTextureVertex>(this);
        }

        private XbimSet<IfcTextureVertex> _TextureVertices;

        /// <summary>
        /// List of texture vertex coordinates, each texture vertex refers to the Cartesian point within the polyloop (corresponding lists). The first coordinate[1] is the S, the second coordinate[2] is the T parameter value. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 3)]
        public XbimSet<IfcTextureVertex> TextureVertices
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TextureVertices;
            }
            set { this.SetModelValue(this, ref _TextureVertices, value, v => TextureVertices = v, "TextureVertices"); }
        }

        private XbimSet<IfcCartesianPoint> _TexturePoints;

        /// <summary>
        ///  Reference to a list of polyloop's defining a face bound of a face within a vertex based geometry. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 3)]
        public XbimSet<IfcCartesianPoint> TexturePoints
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TexturePoints;
            }
            set { this.SetModelValue(this, ref _TexturePoints, value, v => TexturePoints = v, "TexturePoints"); }
        }

        void IPersistIfc.IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _TextureVertices.Add((IfcTextureVertex)value.EntityVal);
                    break;
                case 1:
                    _TexturePoints.Add((IfcCartesianPoint)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        string IPersistIfc.WhereRule()
        {
            return "";
        }

        private IModel _model;
        private int _entityLabel;
        private bool _activated;

        bool IPersistIfcEntity.Activated
        {
            get { return _activated; }
        }

        void IPersistIfcEntity.Activate(bool write)
        {
            lock (this) { if (_model != null && !_activated) _activated = _model.Activate(this, false) > 0; }
            if (write) _model.Activate(this, write);
        }

        void IPersistIfcEntity.Bind(IModel model, int entityLabel, bool activated)
        {
            _model = model;
            _entityLabel = entityLabel;
            _activated = activated;
        }

        IModel IPersistIfcEntity.ModelOf
        {
            get { return _model; }
        }

        int IPersistIfcEntity.EntityLabel
        {
            get { return _entityLabel; }
        }

        [field: NonSerialized] //don't serialize events
        private event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized] //don't serialize event
        private event PropertyChangingEventHandler PropertyChanging;

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

        event System.ComponentModel.PropertyChangedEventHandler System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        event System.ComponentModel.PropertyChangingEventHandler System.ComponentModel.INotifyPropertyChanging.PropertyChanging
        {
            add { PropertyChanging += value; }
            remove { PropertyChanging -= value; }
        }
    }
}
