using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
//using Xbim.IO;
using Xbim.Ifc2x3.Extensions;
using Xbim.XbimExtensions.Interfaces;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Xbim.IO.ViewModels
{
    /// <summary>
    /// Model view for display top level Xbim Model contents and referenced models
    /// </summary>
    public class XbimModelViewModel : IXbimViewModel
    {
        XbimModel xbimModel;
        IfcProject _project;
        private bool _isSelected;
        private bool _isExpanded;
        private ObservableCollection<IXbimViewModel> children;
        public IXbimViewModel CreatingParent { get; set; } 

        public string Name
        {
            get
            {
                // to improve on the user interface experience the classification viewer makes up a name in case the name is empty
                if (_project.Name != string.Empty)
                    return _project.Name;
                else
                    return "Unnamed project";
            }
        }


        public XbimModelViewModel(IfcProject project, IXbimViewModel parent)
        {
            xbimModel = project.ModelOf as XbimModel;
            _project = project;
            this.CreatingParent = parent;
            IEnumerable subs = this.Children; //call this once to preload first level of hierarchy   
        }



        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (children == null)
                {
                    children = new ObservableCollection<IXbimViewModel>();
                    foreach (var item in _project.GetSpatialStructuralElements())
                    {
                        children.Add(new SpatialViewModel(item, this));
                    }
                    foreach (var refModel in xbimModel.ReferencedModels)
                    {
                        children.Add(new XbimRefModelViewModel(refModel, this));
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
            get { return _project.EntityLabel; }
        }

        public IPersistIfcEntity Entity
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
            children.Add(xbimModelViewModel);
            NotifyPropertyChanged("Children");
        }

        public void RemoveRefModel(XbimRefModelViewModel xbimModelViewModel)
        {
            children.Remove(xbimModelViewModel);
            NotifyPropertyChanged("Children");
        }


        public XbimModel Model
        {
            get { return xbimModel; }
        }
    }
}
