#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnectss.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.Kernel;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcRelConnectss
    {
        private readonly IModel _model;

        public IfcRelConnectss(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRelConnects> Items
        {
            get { return this._model.Instances.OfType<IfcRelConnects>(); }
        }

        public IfcRelCoversBldgElementss IfcRelCoversBldgElementss
        {
            get { return new IfcRelCoversBldgElementss(_model); }
        }

        public IfcRelConnectsElementss IfcRelConnectsElementss
        {
            get { return new IfcRelConnectsElementss(_model); }
        }

        public IfcRelReferencedInSpatialStructures IfcRelReferencedInSpatialStructures
        {
            get { return new IfcRelReferencedInSpatialStructures(_model); }
        }

        public IfcRelConnectsPortss IfcRelConnectsPortss
        {
            get { return new IfcRelConnectsPortss(_model); }
        }

        public IfcRelCoversSpacess IfcRelCoversSpacess
        {
            get { return new IfcRelCoversSpacess(_model); }
        }

        public IfcRelServicesBuildingss IfcRelServicesBuildingss
        {
            get { return new IfcRelServicesBuildingss(_model); }
        }

        public IfcRelConnectsStructuralMembers IfcRelConnectsStructuralMembers
        {
            get { return new IfcRelConnectsStructuralMembers(_model); }
        }

        public IfcRelSequences IfcRelSequences
        {
            get { return new IfcRelSequences(_model); }
        }

        public IfcRelConnectsStructuralElements IfcRelConnectsStructuralElements
        {
            get { return new IfcRelConnectsStructuralElements(_model); }
        }

        public IfcRelSpaceBoundarys IfcRelSpaceBoundarys
        {
            get { return new IfcRelSpaceBoundarys(_model); }
        }

        public IfcRelVoidsElements IfcRelVoidsElements
        {
            get { return new IfcRelVoidsElements(_model); }
        }

        public IfcRelFlowControlElementss IfcRelFlowControlElementss
        {
            get { return new IfcRelFlowControlElementss(_model); }
        }

        public IfcRelConnectsStructuralActivitys IfcRelConnectsStructuralActivitys
        {
            get { return new IfcRelConnectsStructuralActivitys(_model); }
        }

        public IfcRelProjectsElements IfcRelProjectsElements
        {
            get { return new IfcRelProjectsElements(_model); }
        }

        public IfcRelConnectsPortToElements IfcRelConnectsPortToElements
        {
            get { return new IfcRelConnectsPortToElements(_model); }
        }

        public IfcRelContainedInSpatialStructures IfcRelContainedInSpatialStructures
        {
            get { return new IfcRelContainedInSpatialStructures(_model); }
        }

        public IfcRelFillsElements IfcRelFillsElements
        {
            get { return new IfcRelFillsElements(_model); }
        }
    }
}