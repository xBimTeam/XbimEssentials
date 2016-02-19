using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.ViewModels
{
    public class IfcProductModelView : IXbimViewModel
    {
        private readonly IIfcProduct _product;
        private bool _isSelected;
        private bool _isExpanded;
        private List<IXbimViewModel> _children;
        public IXbimViewModel CreatingParent { get; set; } 

        public IfcProductModelView(IIfcProduct prod, IXbimViewModel parent)
        {
            CreatingParent = parent;
            _product = prod;
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (_children != null) return _children;

                _children = new List<IXbimViewModel>();
                var breakdown = _product.IsDecomposedBy.ToList();
                if (!breakdown.Any()) return _children;
                
                foreach (var rel in breakdown)
                {
                    foreach (var prod in rel.RelatedObjects.OfType<IIfcProduct>())
                    {
                        _children.Add(new IfcProductModelView(prod, this));
                    }
                }
                return _children;
            }
        }

        public string Name
        {
            get
            {              
                if(!string.IsNullOrWhiteSpace(_product.Name))
                 return string.Format("{0} - {1} #{2}",_product.Name, _product.ExpressType.ExpressName.Substring(3), _product.EntityLabel );
                return string.Format("{0} #{1}", _product.ExpressType.ExpressName.Substring(3), _product.EntityLabel);
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
            get { return _product.EntityLabel; }
        }


        public IPersistEntity Entity
        {
            get { return _product; }
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
            get { return _product.Model; }
        }
    }
}
