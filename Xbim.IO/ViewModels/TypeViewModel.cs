using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.IO.ViewModels
{
    public class TypeViewModel : IXbimViewModel
    {
        XbimModel xbimModel;
        Type type;
        //int spatialContainerLabel;
        private bool _isSelected;
        private bool _isExpanded;

        private List<IXbimViewModel> children;
        public IXbimViewModel CreatingParent { get; set; } 

        public string Name
        {
            get
            {
                return type.Name;
            }
        }

        public TypeViewModel(Type type, XbimModel model)
        {
            this.type = type;
            xbimModel = model;
        }


        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (children == null)
                {
                    children = new List<IXbimViewModel>();
                    var products = xbimModel.Instances.Where<IfcProduct>(p => p.GetType().IsAssignableFrom(this.type));
                    foreach (IfcProduct prod in products)
                        children.Add(new IfcProductModelView(prod, this));
                }
                return children;
            }
        }

        public int EntityLabel
        {
            get { return 0; }
        }


        public XbimExtensions.Interfaces.IPersistIfcEntity Entity
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

        public XbimModel Model
        {
            get { return xbimModel; }
        }
    }
}
