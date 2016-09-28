using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.ViewModels
{
    public class SpatialViewModel : IXbimViewModel
    {
        private readonly IModel _model;
        private readonly IIfcObjectDefinition _spatialStructure;
        private bool _isSelected;
        private bool _isExpanded;
        private List<IXbimViewModel> _children;
        public IXbimViewModel CreatingParent { get; set; } 

        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_spatialStructure.Name))
                    return string.Format("{0} #{1}", _spatialStructure.Name, _spatialStructure.EntityLabel);
                return string.Format("{0} #{1}", _spatialStructure.ExpressType.ExpressName.Substring(3), _spatialStructure.EntityLabel);               
            }
        }
        
        public SpatialViewModel(IIfcSpatialStructureElement spatialStructure, IXbimViewModel parent)
        {
            if (spatialStructure == null) throw new ArgumentNullException("spatialStructure");
            _model = spatialStructure.Model;
            _spatialStructure = spatialStructure;
            CreatingParent = parent;
        }

        public SpatialViewModel(IIfcProject project)
        {
            if (project == null) throw new ArgumentNullException("project");
            _model = project.Model;
            _spatialStructure = project;
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children != null) return _children;

                _children = new List<IXbimViewModel>();
                // list related items of type IfcSpatialStructureElement
                foreach (var aggregate in _spatialStructure.IsDecomposedBy)
                {
                    foreach (var subSpace in aggregate.RelatedObjects.OfType<IIfcSpatialStructureElement>())
                        _children.Add(new SpatialViewModel(subSpace, this));
                }

                // now add any contained elements
                // this will not happen in case item is IfcProject
                var spatialElem = _spatialStructure as IIfcSpatialStructureElement;
                if (spatialElem == null) return _children;

                //Select all the disting type names of elements for this
                foreach (var type in spatialElem.ContainsElements.SelectMany(container=>container.RelatedElements).Select(r=>r.GetType()).Distinct())
                {
                    _children.Add(new ContainedElementsViewModel(spatialElem, type, this));
                }
                return _children;
            }
        }

        public bool HasItems
        {
            get
            {
                return Children.Any();
            }
        }

        public int EntityLabel
        {
            get { return _spatialStructure.EntityLabel; }
        }


        public IPersistEntity Entity
        {
            get { return _spatialStructure; }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                NotifyPropertyChanged("IsExpanded");
            }
        }
        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
        private event PropertyChangedEventHandler PropertyChanged;


        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }
        void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion



        public IModel Model
        {
            get { return _model; }
        }
    }


}
