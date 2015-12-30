using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Federation;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.ViewModels
{
    public class XbimRefModelViewModel : IXbimViewModel
    {
        private readonly IReferencedModel _refModel;
        private bool _isSelected;
        private bool _isExpanded;
        private List<IXbimViewModel> _children;
        public IXbimViewModel CreatingParent { get; set; } 

        public XbimRefModelViewModel(IReferencedModel refModel, IXbimViewModel parent)
        {
            CreatingParent = parent;
            _refModel = refModel;
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<IXbimViewModel>();
                    var project = _refModel.Model.Instances.FirstOrDefault<IIfcProject>();
                    if (project != null)
                    {
                        foreach (var item in project.GetSpatialStructuralElements())
                        {
                            _children.Add(new SpatialViewModel(item, this));
                        }
                    }
                }
                return _children;
            }
        }

        

        public override string ToString()
        {
            return Name;
        }
        public string Name
        {
            get 
            {
                var name = _refModel.Name;
                name = Path.GetFileNameWithoutExtension(name);
                name += " [" + _refModel.Role + "]";
                return name;
            }
        }

        public IReferencedModel RefModel
        {
            get { return _refModel; }
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
            get { return -1; }
        }

        public IPersistEntity Entity
        {
            get { return null; }
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
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion






        public IModel Model
        {
            get { return _refModel.Model; }
        }
    }
}
