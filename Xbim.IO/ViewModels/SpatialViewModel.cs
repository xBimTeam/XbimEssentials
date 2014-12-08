using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Xbim.IO;
using System.Collections.ObjectModel;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.XbimExtensions.Interfaces;
using System.Collections;
using System.ComponentModel;

namespace Xbim.IO.ViewModels
{
    public class SpatialViewModel : IXbimViewModel
    {
        XbimModel xbimModel;
        int spatialStructureLabel;
        private bool _isSelected;
        private bool _isExpanded;
        private List<IXbimViewModel> children;
        public IXbimViewModel CreatingParent { get; set; } 

        public string Name
        {
            get
            {
                IPersistIfcEntity ent = xbimModel.Instances[spatialStructureLabel];
                return ent.ToString();
            }
            set
            {
            }
        }
        
        public SpatialViewModel(IfcSpatialStructureElement spatialStructure, IXbimViewModel parent)
        {
            xbimModel = spatialStructure.ModelOf as XbimModel;
            this.spatialStructureLabel = spatialStructure.EntityLabel;
            CreatingParent = parent;
           // IEnumerable subs = this.Children; //call this once to preload first level of hierarchy   
        }

        public SpatialViewModel(IfcProject project)
        {
            xbimModel = project.ModelOf as XbimModel;
            this.spatialStructureLabel = project.EntityLabel;
           // IEnumerable subs = this.Children; //call this once to preload first level of hierarchy          
        }

        public IEnumerable<IXbimViewModel> Children
        {
            get
            {
                if (children == null)
                {
                    children = new List<IXbimViewModel>();
                    IfcObjectDefinition space = xbimModel.Instances[spatialStructureLabel] as IfcObjectDefinition;
                    if (space != null)
                    {
                        // list related items of type IfcSpatialStructureElement
                        IEnumerable<IfcRelAggregates> AllAggregates = space.IsDecomposedBy.OfType<IfcRelAggregates>();
                        foreach (IfcRelAggregates Aggregate in AllAggregates)
                        {
                            foreach (IfcSpatialStructureElement subSpace in Aggregate.RelatedObjects.OfType<IfcSpatialStructureElement>())
                                children.Add(new SpatialViewModel(subSpace, this));
                        }

                        // now add any contained elements
                        // this will not happen in case item is IfcProject
                        IfcSpatialStructureElement spatialElem = space as IfcSpatialStructureElement;
                        if (spatialElem != null)
                        {
                            //Select all the disting type names of elements for this
                            foreach (var type in spatialElem.ContainsElements.SelectMany(container=>container.RelatedElements).Select(r=>r.GetType()).Distinct())
                            {
                                children.Add(new ContainedElementsViewModel(spatialElem, type, this));
                            }
                        }
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
            get { return spatialStructureLabel; }
        }


        public IPersistIfcEntity Entity
        {
            get { return xbimModel.Instances[spatialStructureLabel]; }
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
