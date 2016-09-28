using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Federation;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.Interfaces;
//using Xbim.IO;

namespace Xbim.Ifc.ViewModels
{
    /// <summary>
    /// Model view for display top level Xbim Model contents and referenced models
    /// </summary>
    public class XbimModelViewModel : IXbimViewModel
    {
        private readonly IModel _model;
        private readonly IIfcProject _project;
        private bool _isSelected;
        private bool _isExpanded;
        private ObservableCollection<IXbimViewModel> _children;
        public IXbimViewModel CreatingParent { get; set; } 

        public string Name
        {
            get
            {
                // to improve on the user interface experience the classification viewer makes up a name in case the name is empty
                if (_project.Name != string.Empty)
                    return _project.Name;
                return "Unnamed project";
            }
        }


        public XbimModelViewModel(IIfcProject project, IXbimViewModel parent)
        {
            _model = project.Model;
            _project = project;
            CreatingParent = parent;
            IEnumerable subs = Children; //call this once to preload first level of hierarchy   
        }



        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new ObservableCollection<IXbimViewModel>();
                    foreach (var item in _project.GetSpatialStructuralElements())
                    {
                        _children.Add(new SpatialViewModel(item, this));
                    }
                    
                    var federation = _model as IFederatedModel;
                    if (federation == null) return _children;
                    
                    foreach (var refModel in federation.ReferencedModels)
                    {
                        _children.Add(new XbimRefModelViewModel(refModel, this));
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
            get { return _project.EntityLabel; }
        }

        public IPersistEntity Entity
        {
            get { return _project; }
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


        public void AddRefModel(XbimRefModelViewModel xbimModelViewModel)
        {
            _children.Add(xbimModelViewModel);
            NotifyPropertyChanged("Children");
        }

        public void RemoveRefModel(XbimRefModelViewModel xbimModelViewModel)
        {
            _children.Remove(xbimModelViewModel);
            NotifyPropertyChanged("Children");
        }


        public IModel Model
        {
            get { return _model; }
        }
    }
}
