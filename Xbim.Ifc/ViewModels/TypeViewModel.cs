using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.ViewModels
{
    public class TypeViewModel : IXbimViewModel
    {
        private readonly IModel _model;
        private readonly Type _type;
        private bool _isSelected;
        private bool _isExpanded;

        private List<IXbimViewModel> _children;
        public IXbimViewModel CreatingParent { get; set; } 

        public string Name
        {
            get
            {
                return _type.Name.Substring(3);
            }
        }

        public TypeViewModel(Type type, IModel model)
        {
            _type = type;
            _model = model;
        }


        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<IXbimViewModel>();
                    var products = _model.Instances.Where<IIfcProduct>(p => p.GetType().IsAssignableFrom(_type));
                    foreach (var prod in products)
                        _children.Add(new IfcProductModelView(prod, this));
                }
                return _children;
            }
        }

        public int EntityLabel
        {
            get { return 0; }
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
