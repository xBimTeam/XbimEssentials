#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelationships.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic; using System.Linq;using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.Kernel;

#endregion

namespace Xbim.XbimExtensions.DataProviders
{
    public class IfcRelationships
    {
        private readonly IModel _model;

        public IfcRelationships(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRelationship> Items
        {
            get { return this._model.Instances.OfType<IfcRelationship>(); }
        }

        public IfcRelDecomposess IfcRelDecomposess
        {
            get { return new IfcRelDecomposess(_model); }
        }

        public IfcRelConnectss IfcRelConnectss
        {
            get { return new IfcRelConnectss(_model); }
        }

        public IfcRelAssignss IfcRelAssignss
        {
            get { return new IfcRelAssignss(_model); }
        }

        public IfcRelAssociatess IfcRelAssociatess
        {
            get { return new IfcRelAssociatess(_model); }
        }

        public IfcRelDefiness IfcRelDefiness
        {
            get { return new IfcRelDefiness(_model); }
        }
    }
}