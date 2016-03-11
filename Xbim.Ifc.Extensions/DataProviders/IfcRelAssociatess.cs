#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAssociatess.cs
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
    public class IfcRelAssociatess
    {
        private readonly IModel _model;

        public IfcRelAssociatess(IModel model)
        {
            this._model = model;
        }

        public IEnumerable<IfcRelAssociates> Items
        {
            get { return this._model.Instances.OfType<IfcRelAssociates>(); }
        }

        public IfcRelAssociatesDocuments IfcRelAssociatesDocuments
        {
            get { return new IfcRelAssociatesDocuments(_model); }
        }

        public IfcRelAssociatesLibrarys IfcRelAssociatesLibrarys
        {
            get { return new IfcRelAssociatesLibrarys(_model); }
        }

        public IfcRelAssociatesProfilePropertiess IfcRelAssociatesProfilePropertiess
        {
            get { return new IfcRelAssociatesProfilePropertiess(_model); }
        }

        public IfcRelAssociatesClassifications IfcRelAssociatesClassifications
        {
            get { return new IfcRelAssociatesClassifications(_model); }
        }

        public IfcRelAssociatesMaterials IfcRelAssociatesMaterials
        {
            get { return new IfcRelAssociatesMaterials(_model); }
        }
    }
}