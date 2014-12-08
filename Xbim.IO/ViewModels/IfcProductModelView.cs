using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ProductExtension;
using System.Collections;
using Xbim.Ifc2x3.Kernel;
using System.ComponentModel;
using System.Collections.ObjectModel;
//using Xbim.IO;
using System.Diagnostics;

namespace Xbim.IO.ViewModels
{
    public class IfcProductModelView : IXbimViewModel
    {
        private IfcProduct product;
        private bool _isSelected;
        private bool _isExpanded;
        private List<IXbimViewModel> children;
        public IXbimViewModel CreatingParent { get; set; } 

        public IfcProductModelView(IfcProduct prod, IXbimViewModel parent)
        {
            CreatingParent = parent;
            this.product = prod;
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (children == null)
                {
                    children = new List<IXbimViewModel>();
                    List<IfcRelDecomposes> breakdown = product.IsDecomposedBy.ToList();

                    if (breakdown.Any())
                    {
                        foreach (var rel in breakdown)
                        {
                            foreach (var prod in rel.RelatedObjects.OfType<IfcProduct>())
                            {
                                children.Add(new IfcProductModelView(prod, this));
                            }
                        }
                    }
                }
                return children;
            }
        }

        public string Name
        {
            get { 
                var tmp = product.ToString(); 
                if (tmp.Trim() == string.Empty) // some applications write a single space in place of the name
                    tmp = string.Format("{0} #{1}", product.IfcType().ToString(), product.EntityLabel);
                return tmp;
            }
        }

        public bool HasItems
        {
            get
            {
                IEnumerable subs = this.Children; //call this once to preload first level of hierarchy          
                return children.Count > 0;
            }
        }

      
        public int EntityLabel
        {
            get { return product.EntityLabel; }
        }


        public XbimExtensions.Interfaces.IPersistIfcEntity Entity
        {
            get { return product; }
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


        public IO.XbimModel Model
        {
            get { return (XbimModel)product.ModelOf; }
        }
    }
}
