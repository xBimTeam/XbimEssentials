using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.IO;
using Xbim.Ifc2x3.Extensions;
using System.ComponentModel;

namespace Xbim.IO.ViewModels
{
    public class GroupViewModel : IXbimViewModel
    {
        private IfcGroup group;
        private bool _isSelected;
        private bool _isExpanded;
        private List<IXbimViewModel> children;

        public IXbimViewModel CreatingParent { get; set; }

        public GroupViewModel(IfcGroup gr, IXbimViewModel parent)
        {
            this.group = gr;
            CreatingParent = parent;
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get {
                if (children == null)
                {
                    children = new List<IXbimViewModel>();
                    IfcRelAssignsToGroup breakdown = group.IsGroupedBy;

                    if (breakdown != null)
                    {
                        foreach (var prod in breakdown.RelatedObjects.OfType<IfcProduct>()) //add products in the group
                        {
                            children.Add(new IfcProductModelView(prod, this));
                        }
                        foreach (var gr in breakdown.RelatedObjects.OfType<IfcGroup>()) //add nested groups
                        {
                            children.Add(new GroupViewModel(gr, this));
                        }
                    }
                }
                return children;
            }
        }

        public string Name
        {
            get { return group.Name; }
        }

        public int EntityLabel
        {
            get { return group.EntityLabel; }
        }

        public XbimExtensions.Interfaces.IPersistIfcEntity Entity
        {
            get { return group; }
        }

        public IO.XbimModel Model
        {
            get { return (XbimModel)group.ModelOf; }
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

        public override string ToString()
        {
            return String.Format("{0}: {1} ({2})", Name, group.Description, group.GetGroupedObjects<IfcProduct>().Count()); 
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
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
    }
}
