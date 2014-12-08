using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Xbim.IO;
using Xbim.Ifc2x3.ExternalReferenceResource;
using System.Collections.ObjectModel;

namespace Xbim.IO.ViewModels
{
    // this class does not seem to be used by any other part of the seolution.
    public class ClassificationViewModel
    {
        XbimModel xbimModel;
        XbimInstanceHandle classificationHandle;
        private ObservableCollection<ClassificationViewModel> subClassifications;

        public string Name
        {
            get
            {
                return xbimModel.Instances[classificationHandle.EntityLabel].ToString();
            }
        }

        public ClassificationViewModel(IfcClassificationItem classification)
        {
            xbimModel = classification.ModelOf as XbimModel;
            this.classificationHandle = new XbimInstanceHandle(this.xbimModel, classification.EntityLabel, classification.GetType());
            IEnumerable<ClassificationViewModel> subs = this.SubClassifications; //call this once to preload first level of hierarchy   
        }


        public ObservableCollection<ClassificationViewModel> SubClassifications
        {
            get
            {
                if (subClassifications == null)
                {
                    subClassifications = new ObservableCollection<ClassificationViewModel>();
                    IfcClassificationItem classification = xbimModel.Instances[classificationHandle.EntityLabel] as IfcClassificationItem;
                    if (classification != null)
                    {
                        IEnumerable<IfcClassificationItemRelationship> rels = classification.IsClassifiedItemIn;
                        foreach (IfcClassificationItemRelationship rel in rels)
                        {
                            foreach (IfcClassificationItem subClassification in rel.RelatedItems)
                                subClassifications.Add(new ClassificationViewModel(subClassification));
                        }
                    }
                }
                return subClassifications;
            }
        }
    }
}