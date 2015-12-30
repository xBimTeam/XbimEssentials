using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.ViewModels
{
    public class ContainedElementsViewModel : IXbimViewModel 
    {
        private readonly IModel _model;
        private readonly Type _type;
        private readonly IIfcSpatialStructureElement _spatialContainer;
        private bool _isSelected;
        private bool _isExpanded;
        public IXbimViewModel CreatingParent { get; set; } 

        private List<IXbimViewModel> _children;

        public string Name
        {
            get
            {
                return _type.Name;
            }
        }


        //public ContainedElementsViewModel(IfcSpatialStructureElement container)
        //{
        //    xbimModel = container.ModelOf as XbimModel;
        //    IEnumerable subs = this.Children; //call this once to preload first level of hierarchy          
        //}

        public ContainedElementsViewModel(IIfcSpatialStructureElement spatialElem, Type type, IXbimViewModel parent)
        {
            _spatialContainer = spatialElem;
            _type = type;
            _model = spatialElem.Model;
            CreatingParent = parent;
        }


        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<IXbimViewModel>();
                    foreach (var rel in _spatialContainer.ContainsElements)
                    {
                        foreach (var prod in rel.RelatedElements.Where(e => e.GetType() == _type))
                            _children.Add(new IfcProductModelView(prod, this));
                    }
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
            get { return _spatialContainer.EntityLabel; }
        }


        public IPersistEntity Entity
        {
            get { return _spatialContainer; }
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
