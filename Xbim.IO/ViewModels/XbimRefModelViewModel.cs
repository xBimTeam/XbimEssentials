using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.Extensions;
using System.Collections;
using System.IO;

namespace Xbim.IO.ViewModels
{
    public class XbimRefModelViewModel : IXbimViewModel
    {
        private IO.XbimReferencedModel refModel;
        private bool _isSelected;
        private bool _isExpanded;
        private List<IXbimViewModel> children;
        public IXbimViewModel CreatingParent { get; set; } 

        public XbimRefModelViewModel(IO.XbimReferencedModel refModel, IXbimViewModel parent)
        {
            this.CreatingParent = parent;
            this.refModel = refModel;
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (children == null)
                {
                    children = new List<IXbimViewModel>();
                    IfcProject project = refModel.Model.IfcProject;
                    if (project != null)
                    {
                        foreach (var item in project.GetSpatialStructuralElements())
                        {
                            children.Add(new SpatialViewModel(item, this));
                        }
                    }
                }
                return children;
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
                string name = this.refModel.Name;
                name = Path.GetFileNameWithoutExtension(name);
                name += " [" + refModel.DocumentInformation.DocumentOwner.RoleName() + "]";
                return name;
            }
        }

        public IO.XbimReferencedModel RefModel
        {
            get { return refModel; }
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
            get { return refModel.DocumentInformation.EntityLabel; }
        }

        public XbimExtensions.Interfaces.IPersistIfcEntity Entity
        {
            get { return refModel.DocumentInformation; }
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
            get { return refModel.Model; }
        }
    }
}
