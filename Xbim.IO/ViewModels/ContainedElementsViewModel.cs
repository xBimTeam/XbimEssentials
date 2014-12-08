using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Xbim.IO;
using System.Collections.ObjectModel;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using System.Collections;
using System.ComponentModel;

namespace Xbim.IO.ViewModels
{
    public class ContainedElementsViewModel : IXbimViewModel 
    {
        XbimModel xbimModel;
        Type type;
        int spatialContainerLabel;
        private bool _isSelected;
        private bool _isExpanded;
        public IXbimViewModel CreatingParent { get; set; } 

        private List<IXbimViewModel> children;

        public string Name
        {
            get
            {
                return type.Name;
            }
        }


        //public ContainedElementsViewModel(IfcSpatialStructureElement container)
        //{
        //    xbimModel = container.ModelOf as XbimModel;
        //    IEnumerable subs = this.Children; //call this once to preload first level of hierarchy          
        //}

        public ContainedElementsViewModel(IfcSpatialStructureElement spatialElem, Type type, IXbimViewModel parent)
        {
            this.spatialContainerLabel = spatialElem.EntityLabel;
            this.type = type;
            this.xbimModel = (XbimModel) spatialElem.ModelOf;
            this.CreatingParent = parent;
        }


        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (children == null)
                {
                    children = new List<IXbimViewModel>();
                    IfcSpatialStructureElement space = xbimModel.Instances[spatialContainerLabel] as IfcSpatialStructureElement;
                    foreach (var rel in space.ContainsElements)
                    {
                        foreach (IfcProduct prod in rel.RelatedElements.Where(e => e.GetType() == type))
                            children.Add(new IfcProductModelView(prod, this));
                    }
                }
                return children;
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
            get { return 0; }
        }


        public XbimExtensions.Interfaces.IPersistIfcEntity Entity
        {
            get { return xbimModel.Instances[spatialContainerLabel]; }
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
