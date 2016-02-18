using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.ViewModels
{
    public class GroupViewModel : IXbimViewModel
    {
        private readonly IIfcGroup _group;
        private bool _isSelected;
        private bool _isExpanded;
        private List<IXbimViewModel> _children;

        public IXbimViewModel CreatingParent { get; set; }

        public GroupViewModel(IIfcGroup gr, IXbimViewModel parent)
        {
            _group = gr;
            CreatingParent = parent;
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get {
                if (_children == null)
                {
                    _children = new List<IXbimViewModel>();
                    foreach (var relAssignsToGroup in _group.IsGroupedBy)
                    {
                        foreach (var prod in relAssignsToGroup.RelatedObjects.OfType<IIfcProduct>()) //add products in the _group
                        {
                            _children.Add(new IfcProductModelView(prod, this));
                        }
                        foreach (var gr in relAssignsToGroup.RelatedObjects.OfType<IIfcGroup>()) //add nested groups
                        {
                            _children.Add(new GroupViewModel(gr, this));
                        }
                    }
                }
                return _children;
            }
        }

        public string Name
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_group.Name))
                    return string.Format("{0} #{1}", _group.Name, _group.EntityLabel);
                return string.Format("{0} #{1}", _group.ExpressType.ExpressName.Substring(3), _group.EntityLabel);
            }
        }

        public int EntityLabel
        {
            get { return _group.EntityLabel; }
        }

        public IPersistEntity Entity
        {
            get { return _group; }
        }

        public IModel Model
        {
            get { return _group.Model; }
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
            return string.Format("{0}: {1} ({2})", Name, _group.Description, _group.GetGroupedObjects<IIfcProduct>().Count()); 
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized] //don't serialize events
        public event PropertyChangedEventHandler PropertyChanged;
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
