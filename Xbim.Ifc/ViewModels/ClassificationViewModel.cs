using System;
using System.Collections.ObjectModel;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.ViewModels
{
    // this class does not seem to be used by any other part of the seolution.
    public class ClassificationViewModel
    {
        private readonly IIfcClassificationReference _classification;
        private readonly IModel _model;
        private ObservableCollection<ClassificationViewModel> _subClassifications;

        public string Name
        {
            get
            {
                if (_classification.Name.HasValue) return _classification.Name;
                if (_classification.Identification.HasValue) return _classification.Identification;
                if (_classification.Description.HasValue) return _classification.Description;
                return "";
            }
        }

        public ClassificationViewModel(IIfcClassificationReference classification)
        {
            if (classification == null) throw new ArgumentNullException("classification");
            _classification = classification;
            _model = classification.Model;
        }


        public ObservableCollection<ClassificationViewModel> SubClassifications
        {
            get
            {
                if (_subClassifications == null)
                {
                    _subClassifications = new ObservableCollection<ClassificationViewModel>();
                   
                        foreach (var subClassification in _classification.HasReferences)
                        {
                            _subClassifications.Add(new ClassificationViewModel(subClassification));
                        }
                }
                return _subClassifications;
            }
        }
    }
}